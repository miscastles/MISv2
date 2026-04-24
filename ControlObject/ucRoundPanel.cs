
using System.Drawing;
using System.Windows.Forms;
using static MIS.Function.AppUtilities;

namespace MIS.ControlObject
{
    public partial class ucRoundPanel : UserControl
    {
        public ucRoundPanel()
        {
            InitializeComponent();
            PanelRadius(this, Radius: 6, BorderColor: Color.FromArgb(64, 64, 64), BorderWidth: 0);
        }
    }
}
