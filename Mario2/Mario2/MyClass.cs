using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mario2
{
    public class UIButton : Button
    {
        protected override bool IsInputKey(Keys keyData)
        {
            return base.IsInputKey(keyData);
        }
        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            base.OnKeyDown(kevent);
        }
    }
}
