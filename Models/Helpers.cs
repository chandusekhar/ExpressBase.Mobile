﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Mobile.Models
{
    public interface INativeHelper
    {
        void CloseApp();
    }

    public interface IToast
    {
        void Show(string message);
    }
}