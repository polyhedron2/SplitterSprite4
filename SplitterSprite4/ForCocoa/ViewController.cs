using System;

using AppKit;
using Foundation;

namespace ForCocoa
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            string sample = Vanilla.Shared.Text();
            this.View.Window.Title = sample;
        }
    }
}
