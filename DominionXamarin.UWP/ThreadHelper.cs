using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominionXamarinForms;

namespace DominionXamarin.UWP
{
    public class UWPThreadHelper : ThreadHelper
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
