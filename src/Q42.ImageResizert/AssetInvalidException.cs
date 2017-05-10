using System;
using System.Collections.Generic;
using System.Text;

namespace Q42.ImageResizert
{
    public class AssetInvalidException : Exception
    {
        public AssetInvalidException() : base("Invalid asset")
        {
        }
    }
}
