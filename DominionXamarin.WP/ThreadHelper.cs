using System;
using System.Threading.Tasks;
using DominionXamarinForms;

namespace DominionXamarin.WP
{
    public class WPThreadHelper : ThreadHelper
    {

        private Task task;
        public override void Go()
        {
            this.task.Start();
        }

        public override void SetFunc(function f)
        {
            this.task = new Task(new Action(f));
        }

        public override void Abort()
        {
            //???
        }
    }
}

