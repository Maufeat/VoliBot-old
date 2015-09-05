using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VoliBots
{
    class exListBoxItem
    {
        private string _title;
        private string _details;
        private string _level;
        private Image _itemImage;
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public exListBoxItem(string id, string title, string details, string level, Image image)
        {
            _id = id;
            _title = title;
            _details = details;
            _level = level;
            _itemImage = image;
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Details
        {
            get { return _details; }
            set { _details = value; }
        }

        public string Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public Image ItemImage
        {
            get { return _itemImage; }
            set { _itemImage = value; }
        }

        public void drawItem(DrawItemEventArgs e, Padding margin,
                             Font titleFont, Font detailsFont, Font levelFont, StringFormat aligment,
                             Size imageSize)
        {
            // if selected, mark the background differently
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, (byte)63, (byte)147, (byte)236)), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.WhiteSmoke, e.Bounds);
            }
            // draw some item separator
            if (e.Index == 0)
            {
                e.Graphics.DrawLine(Pens.Gray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);
            }
            e.Graphics.DrawLine(Pens.Gray, e.Bounds.X, e.Bounds.Y + 50, e.Bounds.X + e.Bounds.Width, e.Bounds.Y + 50);

            // draw item image
            e.Graphics.DrawImage(this.ItemImage, e.Bounds.X + margin.Left, e.Bounds.Y + margin.Top, imageSize.Width, imageSize.Height);

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(170, (byte)0, (byte)0, (byte)0)), e.Bounds.X + margin.Left, e.Bounds.Y + 27 + margin.Top, imageSize.Width, 23);

            // calculate bounds for title text drawing
            Rectangle titleBounds = new Rectangle(e.Bounds.X + margin.Horizontal + imageSize.Width + 3,
                                                  e.Bounds.Y + margin.Top + 5,
                                                  e.Bounds.Width - margin.Right - imageSize.Width - margin.Horizontal,
                                                  (int)titleFont.GetHeight() + 2);

            // calculate bounds for details text drawing
            Rectangle detailBounds = new Rectangle(e.Bounds.X + margin.Horizontal + imageSize.Width + 5,
                                                   e.Bounds.Y + (int)titleFont.GetHeight() + 10 + margin.Vertical + margin.Top,
                                                   e.Bounds.Width - margin.Right - imageSize.Width - margin.Horizontal,
                                                   e.Bounds.Height - margin.Bottom - (int)titleFont.GetHeight() - 15 - margin.Vertical - margin.Top);

            Rectangle levelBounds = new Rectangle(e.Bounds.X, e.Bounds.Y + 27, imageSize.Width, 23);

            // draw underline for details
            e.Graphics.DrawLine(Pens.Black, e.Bounds.X + margin.Horizontal + imageSize.Width + 6, e.Bounds.Y + (int)titleFont.GetHeight() + margin.Vertical + detailBounds.Height + margin.Top, e.Bounds.Width - margin.Right - imageSize.Width - margin.Horizontal + 33, e.Bounds.Y + (int)titleFont.GetHeight() + margin.Vertical + margin.Top + detailBounds.Height);

            // draw the text within the bounds
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            if (this.Level != "")
            {
                e.Graphics.DrawString(this.Level, titleFont, Brushes.White, levelBounds, stringFormat);
            }
            else
            {
                e.Graphics.DrawString("?", titleFont, Brushes.White, levelBounds, stringFormat);
            }

            e.Graphics.DrawString(this.Title, titleFont, Brushes.Black, titleBounds, aligment);
            e.Graphics.DrawString(this.Details, detailsFont, Brushes.Black, detailBounds, aligment);
            // put some focus rectangle
            e.DrawFocusRectangle();

        }

    }
    public partial class exListBox : ListBox
    {

        private Size _imageSize;
        private StringFormat _fmt;
        private Font _titleFont;
        private Font _detailsFont;
        private Font _levelFont;

        public exListBox(Font titleFont, Font detailsFont, Font levelFont, Size imageSize,
                         StringAlignment aligment, StringAlignment lineAligment)
        {
            _titleFont = titleFont;
            _detailsFont = detailsFont;
            _levelFont = levelFont;
            _imageSize = imageSize;
            this.ItemHeight = _imageSize.Height + this.Margin.Vertical;
            _fmt = new StringFormat();
            _fmt.Alignment = aligment;
            _fmt.LineAlignment = lineAligment;
            _titleFont = titleFont;
            _detailsFont = detailsFont;
            _levelFont = levelFont;
        }

        public exListBox()
        {
            InitializeComponent();
            _imageSize = new Size(50, 50);
            this.ItemHeight = _imageSize.Height + this.Margin.Vertical;
            _fmt = new StringFormat();
            _fmt.Alignment = StringAlignment.Near;
            _fmt.LineAlignment = StringAlignment.Near;
            _titleFont = new Font(this.Font, FontStyle.Bold);
            _detailsFont = new Font(this.Font, FontStyle.Regular);
            _levelFont = new Font(this.Font, FontStyle.Regular);

        }


        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // prevent from error Visual Designer
            if (this.Items.Count > 0)
            {
                exListBoxItem item = (exListBoxItem)this.Items[e.Index];
                item.drawItem(e, this.Margin, _titleFont, _detailsFont, _levelFont, _fmt, this._imageSize);
            }
        }
        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            e.ItemHeight = 51;
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}