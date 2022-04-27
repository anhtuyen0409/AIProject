using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnTTNT.entity
{
    class OCo
    {
        public const int chieuRong = 28;
        public const int chieuCao = 28;

        //dòng cột để biết ô cờ đang ở dòng nào cột nào trên bàn cờ
        private int dong;
        public int Dong
        {
            set { dong = value; }
            get { return dong; }
        }
        private int cot;
        public int Cot
        {
            set { cot = value; }
            get { return cot; }
        }

        //point để lưu lại vị trí của ô cờ trên bàn cờ
        private Point viTri;
        public Point ViTri
        {
            set { viTri = value; }
            get { return viTri; }
        }

        //để biết được ô cờ của ai đánh
        private int soHuu;
        public int SoHuu
        {
            set { soHuu = value; }
            get { return soHuu; }
        }
        public OCo()
        {

        }
        public OCo(int dong, int cot, Point viTri, int soHuu)
        {
            this.dong = dong;
            this.cot = cot;
            this.viTri = viTri;
            this.soHuu = soHuu;
        }
    }
}
