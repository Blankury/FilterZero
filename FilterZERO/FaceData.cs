using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FilterZERO
{
    class FaceData
    {
        public string PersonName { get; set; }
        public Image<Gray, byte> FaceImage { get; set; }
    }
}
