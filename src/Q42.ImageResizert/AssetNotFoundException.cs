using System;
using System.Collections.Generic;
using System.Text;

namespace Q42.ImageResizert
{
    public class AssetNotFoundException : Exception
    {
        public AssetNotFoundException() : base("Asset could not be found")
        {
        }
    }
}
