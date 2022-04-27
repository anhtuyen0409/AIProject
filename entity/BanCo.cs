using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnTTNT.entity
{
    class BanCo
    {
        private int soDong;
        private int soCot;
        public int SoDong
        {
            get { return soDong; }
        }
        public int SoCot
        {
            get { return soCot; }
        }
        public BanCo()
        {
            soDong = 0;
            soCot = 0;
        }

        public BanCo(int sd, int sc)
        {
            soDong = sd;
            soCot = sc;
        }

        public void veBanCo(Graphics g)
        {
            //vẽ các đường thẳng dọc
            for(int i=0; i<=soCot; i++)
            {
                //x1,y1,x2,y2 y1=0 - đầu bàn cờ , y2=soDong*chieuCao=560 - cuối bàn cờ
                g.DrawLine(CaroChess.pen, i*OCo.chieuRong, 0, i*OCo.chieuRong, soDong*OCo.chieuCao);
            }
            //vẽ các đường thẳng ngang
            for (int j=0; j<= soDong; j++)
            {
                g.DrawLine(CaroChess.pen, 0, j * OCo.chieuCao, soCot*OCo.chieuRong, j*OCo.chieuCao);
            }
        }

        public void veQuanCo(Graphics g, Point point, SolidBrush sb)
        {
            g.FillEllipse(sb, point.X + 2, point.Y + 2, OCo.chieuRong - 4, OCo.chieuCao - 4);
        }

        public void xoaQuanCo(Graphics g, Point point, SolidBrush sb)
        {
            //point.X + 1, point.Y + 1, OCo.chieuRong - 2, OCo.chieuCao - 2 => tránh bị xóa đường viền
            g.FillRectangle(sb, point.X + 1, point.Y + 1, OCo.chieuRong - 2, OCo.chieuCao - 2);
        }
    }
}
