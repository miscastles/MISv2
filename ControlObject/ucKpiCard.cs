using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MIS.ControlObject
{
    public partial class ucKpiCard : UserControl
    {
        public ucKpiCard()
        {
            InitializeComponent();

            // Default values
            Title = "TITLE";
            Value = "0";
            Description = "Description";
        }

        #region Properties

        [Category("KPI")]
        [Description("Title displayed on the KPI card.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get
            {
                return lblTitle.Text;
            }
            set
            {
                lblTitle.Text = value;
            }
        }

        [Category("KPI")]
        [Description("Main value displayed on the KPI card.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Visible)]
        public string Value
        {
            get
            {
                return lblValue.Text;
            }
            set
            {
                lblValue.Text = value;
            }
        }

        [Category("KPI")]
        [Description("Description displayed below the main value.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Visible)]
        public string Description
        {
            get
            {
                return lblDescription.Text;
            }
            set
            {
                lblDescription.Text = value;
            }
        }

        [Category("KPI")]
        [Description("Color of the main KPI value.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Visible)]
        public Color ValueColor
        {
            get
            {
                return lblValue.ForeColor;
            }
            set
            {
                lblValue.ForeColor = value;
            }
        }

        [Category("KPI")]
        [Description("Color of the KPI title.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Visible)]
        public Color TitleColor
        {
            get
            {
                return lblTitle.ForeColor;
            }
            set
            {
                lblTitle.ForeColor = value;
            }
        }

        [Category("KPI")]
        [Description("Color of the KPI description.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Visible)]
        public Color DescriptionColor
        {
            get
            {
                return lblDescription.ForeColor;
            }
            set
            {
                lblDescription.ForeColor = value;
            }
        }

        #endregion

        #region Methods

        public void SetKpi(
            string title,
            string value,
            string description)
        {
            Title = title;
            Value = value;
            Description = description;
        }

        public void Clear()
        {
            Title = "TITLE";
            Value = "0";
            Description = "Description";
        }


        #endregion
    }
}