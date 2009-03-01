#region Using directives

using System;


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
