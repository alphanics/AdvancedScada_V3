﻿
using AdvancedScada;
using AdvancedScada.Controls;
using AdvancedScada.Controls.DialogEditor;
using AdvancedScada.Controls.Licenses;
using AdvancedScada.Controls.Motor;
using AdvancedScada.DriverBase;
using AdvancedScada.DriverBase.Client;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AdvancedScada.Controls.Hydraulic
{
    public class HMIHydraulicCylinder : Control
    {
        public event EventHandler ValueChanged;
        #region متغيرات

        private Bitmap OffImage;

        private Bitmap OnImage;

        private Bitmap RodImage;

        private float ImageRatio;

        private Rectangle TextRectangle;

        private SolidBrush TextBrush;

        private StringFormat sf;

        private bool m_Value;

        private RotateFlipType m_Rotation;

        private bool BackNeedsRefreshed;

        private Bitmap _backBuffer;

        private bool LastValue;

        private Bitmap FormBitmap;
        #endregion
        #region خصائص
        //* This is necessary to make the background draw correctly
        //*  http://www.bobpowell.net/transcontrols.htm
        //*part of the transparent background code
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams cp = base.CreateParams;
                cp.ExStyle |= 32;
                return cp;
            }
        }

        public RotateFlipType Rotation
        {
            get
            {
                return this.m_Rotation;
            }
            set
            {
                this.m_Rotation = value;
                this.BackNeedsRefreshed = true;
                this.RefreshImage();
            }
        }

        public bool Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                if (value != this.m_Value)
                {
                    this.m_Value = value;
                    this.Invalidate();
                    EventArgs empty = EventArgs.Empty;
                    this.OnValueChanged(ref empty);
                }
            }
        }

        #endregion
        #region المشيدات والمهدمات
        public HMIHydraulicCylinder()
        {

            this.TextRectangle = new Rectangle();
            this.sf = new StringFormat();
            this.m_Rotation = RotateFlipType.RotateNoneFlipNone;
            if ((base.ForeColor == Color.FromKnownColor(KnownColor.ControlText)) || (this.ForeColor == Color.FromArgb(0, 0, 0, 0)))
            {
                this.ForeColor = Color.LightGray;
            }
        }


        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this._backBuffer != null)
                {
                    this._backBuffer.Dispose();
                }
                if (this.OffImage != null)
                {
                    this.OffImage.Dispose();
                }
                if (this.OnImage != null)
                {
                    this.OnImage.Dispose();
                }
                if (this.sf != null)
                {
                    this.sf.Dispose();
                }
                if (this.TextBrush != null)
                {
                    this.TextBrush.Dispose();
                }
                if (this.FormBitmap != null)
                {
                    this.FormBitmap.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion
        #region الطرق
        private void GetImageBehindRod()
        {
            if (this.Parent != null && this.Width > 0 && this.Height > 0)
            {
                bool visibleHold = this.Visible;
                this.Visible = false;
                this.Refresh();
                this.FormBitmap = new Bitmap(this.Width, this.Height);
                int m_width = this.Parent.Width;
                Size m_clientSize = this.Parent.ClientSize;
                int borderwidth = Convert.ToInt32((double)(m_width - m_clientSize.Width) / 2);
                int m_height = this.Parent.Height;
                m_clientSize = this.Parent.ClientSize;
                int height1 = (m_height - m_clientSize.Height) - borderwidth;
                Bitmap bitmap = new Bitmap(this.Parent.Width, this.Parent.Height);
                try
                {
                    Control m_parent = this.Parent;
                    Rectangle rectangle = new Rectangle(0, 0, this.Parent.Width, this.Parent.Height);
                    m_parent.DrawToBitmap(bitmap, rectangle);
                }
                catch
                {

                }
                int width1 = this.Width - 1;
                for (int i = 0; i <= width1; i++)
                {
                    int m_Height1 = this.Height - 1;
                    for (int j = 0; j <= m_Height1; j++)
                    {
                        Bitmap m_formBitmap = this.FormBitmap;
                        int x = (i + this.Location.X) + borderwidth;
                        Point location_Renamed = this.Location;
                        m_formBitmap.SetPixel(i, j, bitmap.GetPixel(x, (j + location_Renamed.Y) + height1));
                    }
                }
                this.Visible = visibleHold;
            }
        }

        private void GetImageBehindRodX()
        {
            int num = 0;
            bool visible_Renamed = this.Visible;
            if (this.Parent != null)
            {
                this.Visible = false;
                this.Refresh();
                if (this.FormBitmap != null)
                {
                    this.FormBitmap.Dispose();
                }
                this.FormBitmap = new Bitmap(400, this.RodImage.Height);
                int width_Renamed = this.Parent.ClientSize.Width;
                Size clientSize_Renamed = this.Parent.ClientSize;
                Bitmap bitmap = new Bitmap(width_Renamed, clientSize_Renamed.Height);
                Control parent_Renamed = this.Parent;
                Rectangle rectangle = new Rectangle(0, 0, this.Parent.Width, this.Parent.Height);
                parent_Renamed.DrawToBitmap(bitmap, rectangle);
                int width1 = this.Parent.Width;
                clientSize_Renamed = this.Parent.ClientSize;
                int num1 = Convert.ToInt32((double)(width1 - clientSize_Renamed.Width) / 2);
                int height_Renamed = this.Parent.Height;
                clientSize_Renamed = this.Parent.ClientSize;
                int height1 = (height_Renamed - clientSize_Renamed.Height) - num1;
                int num2 = (Convert.ToInt32((double)this.OnImage.Height / 2) - Convert.ToInt32((double)this.RodImage.Height / 2)) - 1;
                int num3 = 399;
                int width2 = this.FormBitmap.Width - 1;
                int num4 = num3 - num1;
                Point location_Renamed = this.Location;
                num3 = Math.Min(width2, (num4 - location_Renamed.X) - num);
                int height2 = this.RodImage.Height - 1;
                int height3 = this.FormBitmap.Height - 1;
                location_Renamed = this.Location;
                height2 = Math.Min(height3, (height2 + location_Renamed.Y + height1) + num2);
                int num5 = num3;
                for (int i = 0; i <= num5; i++)
                {
                    int num6 = height2;
                    for (int j = 0; j <= num6; j++)
                    {
                        Bitmap formBitmap_Renamed = this.FormBitmap;
                        int num7 = i + num1;
                        location_Renamed = this.Location;
                        int x = (num7 + location_Renamed.X) + num;
                        Point point = this.Location;
                        formBitmap_Renamed.SetPixel(i, j, bitmap.GetPixel(x, (j + point.Y + height1) + num2));
                    }
                }
                this.Visible = visible_Renamed;
            }
        }
        private void RefreshImage()
        {
            float WidthRatio = (float)this.Width / (float)(Properties.Resources.HydraulicCylinder.Width + 400);
            float HeightRatio = (float)this.Height / (float)Properties.Resources.HydraulicCylinder.Height;
            if (this.m_Rotation == RotateFlipType.Rotate90FlipNone || this.m_Rotation == RotateFlipType.Rotate270FlipNone || this.m_Rotation == RotateFlipType.Rotate90FlipX || this.m_Rotation == RotateFlipType.Rotate270FlipX || this.m_Rotation == RotateFlipType.Rotate270FlipX || this.m_Rotation == RotateFlipType.Rotate90FlipX || this.m_Rotation == RotateFlipType.Rotate270FlipNone || this.m_Rotation == RotateFlipType.Rotate90FlipNone)
            {
                WidthRatio = (float)this.Width / (float)Properties.Resources.HydraulicCylinder.Height;
                HeightRatio = (float)this.Height / (float)(Properties.Resources.HydraulicCylinder.Width + 400);
            }
            if (WidthRatio >= HeightRatio)
            {
                this.ImageRatio = HeightRatio;
            }
            else
            {
                this.ImageRatio = WidthRatio;
            }
            if (this.ImageRatio > 0.0F)
            {
                if (this.OnImage != null)
                {
                    this.OnImage.Dispose();
                }
                this.OnImage = new Bitmap(Convert.ToInt32((float)(Properties.Resources.HydraulicCylinder.Width + 400) * this.ImageRatio), Convert.ToInt32((float)Properties.Resources.HydraulicCylinder.Height * this.ImageRatio));
                Graphics gr_dest = Graphics.FromImage(this.OnImage);
                gr_dest.DrawImage(Properties.Resources.HydraulicCylinderRod, 0.0F, 91.0F * this.ImageRatio, (float)Convert.ToInt32((float)Properties.Resources.HydraulicCylinderRod.Width * this.ImageRatio), (float)Convert.ToInt32((float)Properties.Resources.HydraulicCylinderRod.Height * this.ImageRatio));
                gr_dest.DrawImage(Properties.Resources.HydraulicCylinder, 400.0F * this.ImageRatio, 0.0F, (float)Convert.ToInt32((float)Properties.Resources.HydraulicCylinder.Width * this.ImageRatio), (float)Convert.ToInt32((float)Properties.Resources.HydraulicCylinder.Height * this.ImageRatio));
                this.OnImage.RotateFlip(this.m_Rotation);
                if (this.RodImage != null)
                {
                    this.RodImage.Dispose();
                }
                this.RodImage = new Bitmap(Convert.ToInt32((float)(Properties.Resources.HydraulicCylinder.Width + 400) * this.ImageRatio), Convert.ToInt32((float)Properties.Resources.HydraulicCylinder.Height * this.ImageRatio));
                gr_dest = Graphics.FromImage(this.RodImage);
                gr_dest.DrawImage(Properties.Resources.HydraulicCylinderRod, 0.0F, 91.0F * this.ImageRatio, (float)Convert.ToInt32((float)Properties.Resources.HydraulicCylinderRod.Width * this.ImageRatio), (float)Convert.ToInt32((float)Properties.Resources.HydraulicCylinderRod.Height * this.ImageRatio));
                this.RodImage.RotateFlip(this.m_Rotation);
                if (this.OffImage != null)
                {
                    this.OffImage.Dispose();
                }
                this.OffImage = new Bitmap(Convert.ToInt32((float)(Properties.Resources.HydraulicCylinder.Width + 400) * this.ImageRatio), Convert.ToInt32((float)Properties.Resources.HydraulicCylinder.Height * this.ImageRatio));
                gr_dest = Graphics.FromImage(this.OffImage);
                gr_dest.DrawImage(Properties.Resources.HydraulicCylinderRod, 285.0F * this.ImageRatio, 91.0F * this.ImageRatio, (float)Convert.ToInt32((float)Properties.Resources.HydraulicCylinderRod.Width * this.ImageRatio), (float)Convert.ToInt32((float)Properties.Resources.HydraulicCylinderRod.Height * this.ImageRatio));
                gr_dest.DrawImage(Properties.Resources.HydraulicCylinder, 400.0F * this.ImageRatio, 0.0F, (float)Convert.ToInt32((float)Properties.Resources.HydraulicCylinder.Width * this.ImageRatio), (float)Convert.ToInt32((float)Properties.Resources.HydraulicCylinder.Height * this.ImageRatio));
                this.OffImage.RotateFlip(this.m_Rotation);
                gr_dest.Dispose();
                if (this.m_Rotation == RotateFlipType.Rotate180FlipNone || this.m_Rotation == RotateFlipType.RotateNoneFlipX)
                {
                    this.TextRectangle.X = 0;
                    this.TextRectangle.Y = 1;
                    this.TextRectangle.Width = Convert.ToInt32(Math.Truncate(Math.Round((double)this.OnImage.Width * 0.55)));
                    this.TextRectangle.Height = this.OnImage.Height - 2;
                }
                else if (this.m_Rotation == RotateFlipType.Rotate270FlipNone || this.m_Rotation == RotateFlipType.Rotate270FlipX)
                {
                    this.TextRectangle.Y = 0;
                    this.TextRectangle.X = 1;
                    this.TextRectangle.Width = this.OnImage.Width - 2;
                    this.TextRectangle.Height = Convert.ToInt32(Math.Truncate(Math.Round((double)this.OnImage.Height * 0.55)));
                }
                else if (!(this.m_Rotation == RotateFlipType.Rotate90FlipNone || this.m_Rotation == RotateFlipType.Rotate90FlipX))
                {
                    this.TextRectangle.X = Convert.ToInt32(Math.Truncate(Math.Round((double)this.OnImage.Width * 0.45)));
                    this.TextRectangle.Y = 1;
                    this.TextRectangle.Width = Convert.ToInt32(Math.Truncate(Math.Round((double)this.OnImage.Width * 0.55)));
                    this.TextRectangle.Height = this.OnImage.Height - 2;
                }
                else
                {
                    this.TextRectangle.Y = Convert.ToInt32(Math.Truncate(Math.Round((double)this.OnImage.Height * 0.45)));
                    this.TextRectangle.X = 1;
                    this.TextRectangle.Width = this.OnImage.Width - 2;
                    this.TextRectangle.Height = Convert.ToInt32(Math.Truncate(Math.Round((double)this.OnImage.Height * 0.55)));
                }
                this.sf.Alignment = StringAlignment.Center;
                this.sf.LineAlignment = StringAlignment.Center;
                if (this.TextBrush == null)
                {
                    this.TextBrush = new SolidBrush(base.ForeColor);
                }
                if (this._backBuffer != null)
                {
                    this._backBuffer.Dispose();
                }
                this._backBuffer = new Bitmap(this.Width, this.Height);
                this.Invalidate();
            }
        }
        #endregion
        #region اعادة تعريف الاحداث
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            if (this.TextBrush != null)
            {
                this.TextBrush.Color = base.ForeColor;
            }
            else
            {
                this.TextBrush = new SolidBrush(base.ForeColor);
            }
            this.Invalidate();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            this.GetImageBehindRod();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle();
            if (!(this.OffImage == null || this.OnImage == null || this._backBuffer == null || this.TextBrush == null || (this.TextRectangle == rectangle) || this.sf == null))
            {
                if (this.FormBitmap == null)
                {
                    this.GetImageBehindRod();
                }
                using (Graphics g = Graphics.FromImage(this._backBuffer))
                {
                    if (!this.m_Value)
                    {
                        g.DrawImage(this.FormBitmap, 0, 0);
                        g.DrawImage(this.OffImage, 0, 0);
                    }
                    else
                    {
                        g.DrawImage(this.FormBitmap, 0, 0);
                        g.DrawImage(this.OnImage, 0, 0);
                    }
                    if ((base.Text == null || string.Compare(base.Text, string.Empty) == 0) ? false : true)
                    {
                        if (this.TextBrush.Color != base.ForeColor)
                        {
                            this.TextBrush.Color = base.ForeColor;
                        }
                        g.DrawString(base.Text, base.Font, this.TextBrush, this.TextRectangle, this.sf);
                    }
                    e.Graphics.DrawImage(this._backBuffer, 0, 0);
                    this.LastValue = this.m_Value;
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.BackNeedsRefreshed)
            {
                base.OnPaintBackground(e);
                this.BackNeedsRefreshed = false;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.RefreshImage();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.GetImageBehindRod();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.Invalidate();
        }
        #endregion
        #region اطلاق الحدث
        protected virtual void OnValueChanged(ref EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        #endregion
        #region PLC


        public bool HoldTimeMet;
        private int m_MaximumHoldTime = 3000;
        private int m_MinimumHoldTime = 500;
        private OutputType m_OutputType = OutputType.MomentarySet;

        //*****************************************
        //* Property - Address in PLC to Link to
        //*****************************************
        private string m_PLCAddressClick = string.Empty;


        //*****************************************
        //* Property - Address in PLC to Link to
        //*****************************************
        private string m_PLCAddressText = string.Empty;

        //*****************************************
        //* Property - Address in PLC to Link to
        //*****************************************
        private string m_PLCAddressValue = string.Empty;

        //*****************************************
        //* Property - Address in PLC to Link to
        //*****************************************
        private string m_PLCAddressVisible = string.Empty;

        //**********************************************************************
        //* If output type is set to write value, then write this value to PLC
        //**********************************************************************

        //*****************************************
        //* Property - Hold time before bit reset
        //*****************************************
        private readonly Timer MaxHoldTimer = new Timer();

        //*****************************************
        //* Property - Hold time before bit reset
        //*****************************************
        private readonly Timer MinHoldTimer = new Timer();
        private readonly bool MouseIsDown = false;


        //***************************************
        //* Call backs for returned data
        //***************************************
        private string OriginalText;


        [Category("PLC Properties")]
        [Editor(typeof(TestDialogEditor), typeof(UITypeEditor))]
        public string PLCAddressText
        {
            get { return m_PLCAddressText; }
            set
            {
                if (m_PLCAddressText != value)
                {
                    m_PLCAddressText = value;

                    try
                    {
                        //* When address is changed, re-subscribe to new address
                        if (string.IsNullOrEmpty(m_PLCAddressText) || string.IsNullOrWhiteSpace(m_PLCAddressText) ||
                            Licenses.LicenseManager.IsInDesignMode) return;
                        var bd = new Binding("Text", TagCollectionClient.Tags[m_PLCAddressValue], "Text", true);
                        DataBindings.Add(bd);
                    }
                    catch (Exception ex)
                    {
                        DisplayError(ex.Message);
                    }
                }
            }
        }

        [Category("PLC Properties")]
        [Editor(typeof(TestDialogEditor), typeof(UITypeEditor))]
        public string PLCAddressVisible
        {
            get { return m_PLCAddressVisible; }
            set
            {
                if (m_PLCAddressVisible != value)
                {
                    m_PLCAddressVisible = value;

                    try
                    {
                        // If Not String.IsNullOrEmpty(m_PLCAddressVisible) Then
                        //* When address is changed, re-subscribe to new address
                        if (string.IsNullOrEmpty(m_PLCAddressVisible) ||
                            string.IsNullOrWhiteSpace(m_PLCAddressVisible) || Licenses.LicenseManager.IsInDesignMode) return;
                        var bd = new Binding("Visible", TagCollectionClient.Tags[m_PLCAddressVisible], "Visible", true);
                        DataBindings.Add(bd);
                        //End If
                    }
                    catch (Exception ex)
                    {
                        DisplayError(ex.Message);
                    }
                }
            }
        }

        [Category("PLC Properties")]
        [Editor(typeof(TestDialogEditor), typeof(UITypeEditor))]
        public string PLCAddressValue
        {
            get { return m_PLCAddressValue; }
            set
            {
                if (m_PLCAddressValue != value)
                {
                    m_PLCAddressValue = value;

                    try
                    {
                        //* When address is changed, re-subscribe to new address
                        if (string.IsNullOrEmpty(m_PLCAddressValue) || string.IsNullOrWhiteSpace(m_PLCAddressValue) ||
                            Licenses.LicenseManager.IsInDesignMode) return;
                        var bd = new Binding("Value", TagCollectionClient.Tags[m_PLCAddressValue], "Value", true);
                        DataBindings.Add(bd);
                    }
                    catch (Exception ex)
                    {
                        DisplayError(ex.Message);
                    }
                }
            }
        }

        [Category("PLC Properties")]
        [Editor(typeof(TestDialogEditor), typeof(UITypeEditor))]
        public string PLCAddressClick
        {
            get { return m_PLCAddressClick; }
            set
            {
                if (m_PLCAddressClick != value) m_PLCAddressClick = value;
            }
        }

        public OutputType OutputType
        {
            get { return m_OutputType; }
            set { m_OutputType = value; }
        }

        [DefaultValue(false)]
        public bool SuppressErrorDisplay { get; set; }

        [Category("PLC Properties")]
        public int MinimumHoldTime
        {
            get { return m_MinimumHoldTime; }
            set
            {
                m_MinimumHoldTime = value;
                if (value > 0) MinHoldTimer.Interval = value;
            }
        }

        [Category("PLC Properties")]
        public int MaximumHoldTime
        {
            get { return m_MaximumHoldTime; }
            set
            {
                m_MaximumHoldTime = value;
                if (value > 0) MaxHoldTimer.Interval = value;
            }
        }

        [Category("PLC Properties")]
        public int ValueToWrite { get; set; }


        private void ReleaseValue()
        {
            try
            {
                switch (OutputType)
                {
                    case OutputType.MomentarySet:
                        WCFChannelFactory.Write(PLCAddressClick, Convert.ToString(false));
                        break;
                    case OutputType.MomentaryReset:
                        WCFChannelFactory.Write(PLCAddressClick, Convert.ToString(true));
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HoldTimer_Tick(object sender, EventArgs e)
        {
            MinHoldTimer.Enabled = false;
            HoldTimeMet = true;
            if (!MouseIsDown) ReleaseValue();
        }

        private void MaxHoldTimer_Tick(object sender, EventArgs e)
        {
            MaxHoldTimer.Enabled = false;
            ReleaseValue();
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);


            if (!string.IsNullOrWhiteSpace(m_PLCAddressClick) & Enabled && PLCAddressClick != null)
            {
                try
                {
                    switch (OutputType)
                    {
                        case OutputType.MomentarySet:
                            WCFChannelFactory.Write(m_PLCAddressClick, "1");
                            break;
                        case OutputType.MomentaryReset:
                            WCFChannelFactory.Write(m_PLCAddressClick, "0");
                            break;
                        case OutputType.SetTrue:
                            WCFChannelFactory.Write(m_PLCAddressClick, "1");
                            break;
                        case OutputType.SetFalse:
                            WCFChannelFactory.Write(m_PLCAddressClick, "0");
                            break;
                        case OutputType.Toggle:

                            var CurrentValue = Value;
                            if (CurrentValue)
                                WCFChannelFactory.Write(m_PLCAddressClick, "0");
                            else
                                WCFChannelFactory.Write(m_PLCAddressClick, "1");
                            break;
                        default:

                            break;
                    }
                }
                catch (Exception ex)
                {
                    DisplayError("WRITE FAILED!" + ex.Message);
                }

                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!string.IsNullOrWhiteSpace(m_PLCAddressClick) & Enabled)
            {
                try
                {
                    switch (OutputType)
                    {
                        case OutputType.MomentarySet:
                            WCFChannelFactory.Write(m_PLCAddressClick, "0");
                            break;
                        case OutputType.MomentaryReset:
                            WCFChannelFactory.Write(m_PLCAddressClick, "1");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    DisplayError("WRITE FAILED!" + ex.Message);
                }

                Invalidate();
            }
        }
        #endregion
        #region "Error Display"

        //********************************************************
        //* Show an error via the text property for a short time
        //********************************************************
        private Timer ErrorDisplayTime;

        private void DisplayError(string ErrorMessage)
        {
            if (!SuppressErrorDisplay)
            {
                if (ErrorDisplayTime == null)
                {
                    ErrorDisplayTime = new Timer();
                    ErrorDisplayTime.Tick += ErrorDisplay_Tick;
                    ErrorDisplayTime.Interval = 5000;
                }

                //* Save the text to return to
                if (!ErrorDisplayTime.Enabled) OriginalText = Text;

                ErrorDisplayTime.Enabled = true;

                Text = ErrorMessage;
            }
        }

        //**************************************************************************************
        //* Return the text back to its original after displaying the error for a few seconds.
        //**************************************************************************************
        private void ErrorDisplay_Tick(object sender, EventArgs e)
        {
            Text = OriginalText;

            if (ErrorDisplayTime != null)
            {
                ErrorDisplayTime.Enabled = false;
                ErrorDisplayTime.Dispose();
                ErrorDisplayTime = null;
            }
        }

        #endregion
    }


}