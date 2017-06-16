using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using RealSense;
using RealSense.Emotions;

namespace RealSense
{
    /**
     * this ist the main class. It start the model and adds all modules to our Model. 
     * 
     * @author Tanja Witke, David Rosenbusch
     * 
     * Interpretation:         0 = Normal
     *                       100 = Lip up
     */
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Model model = new Model(true);
            RSModule.Init(model);

            model.AddModule(new FaceRecorder());
            model.AddModule(new FaceTrackerModule(null));
            // Default Modules
            model.AddModule(new Gauge_Module());
            Application.Run(new CameraView(model, true));
        }
    }
}
