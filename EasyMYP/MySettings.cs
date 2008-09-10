using System;
using System.Collections.Generic;
using System.Text;

namespace EasyMYP
{
    using System;
    using System.Configuration;
    using System.Drawing;

    public class MySettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue("white")]
        public Color BackgroundColor
        {
            get
            {
                return ((Color)this["BackgroundColor"]);
            }
            set
            {
                this["BackgroundColor"] = (Color)value;
            }
        }
    }
}
