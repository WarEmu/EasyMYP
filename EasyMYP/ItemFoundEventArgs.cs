#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace EasyMYP {
  public class ItemFoundEventArgs : EventArgs {
    private int _index;
    public ItemFoundEventArgs(int index) {
      _index = index;
    }
    public int Index {
      get { return _index; }
    }
  }
}
