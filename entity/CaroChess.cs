using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnTTNT.entity
{
    public enum KETTHUC
    {
        HoaCo,
        Player1,
        Player2,
        Computer,
        You
    }
    class CaroChess
    {
        //loại bút vẽ
        public static Pen pen;
        public static SolidBrush sbWhite;
        public static SolidBrush sbBlack;
        public static SolidBrush sbDarkGray;
        

        private BanCo banCo;
        private OCo[,] arrOCo;
        private int luotDi;
        private bool ready;
        private int cheDoChoi;

        //dùng Stack để lưu trữ các nước cờ đã đi
        private Stack<OCo> stkCacNuocDaDi;

        //dùng Stack để lưu trữ các nước cờ đã đi nhưng bị undo trước đó
        private Stack<OCo> stkCacNuocUndo;
        private KETTHUC ketThuc;
        private Random rd = new Random();//random ô cờ máy sẽ đánh đầu tiên

        
        public int CheDoChoi
        {
            get { return cheDoChoi; }
        }
        public bool sanSang
        {
            get { return ready; }
        }

        public CaroChess()
        {
            pen = new Pen(Color.Black);
            sbWhite = new SolidBrush(Color.White);
            sbBlack = new SolidBrush(Color.Black);
            sbDarkGray = new SolidBrush(Color.DarkGray);

            //khởi tạo bàn cờ (20 dòng, 25 cột)
            banCo = new BanCo(20, 25);

            //khởi tạo mảng ô cờ
            arrOCo = new OCo[banCo.SoDong, banCo.SoCot];
            stkCacNuocDaDi = new Stack<OCo>();
            stkCacNuocUndo = new Stack<OCo>();
            luotDi = 1; //người đi đầu tiên
        }




        #region Các thao tác đánh cờ, vẽ bàn cờ
        public void veBanCo(Graphics g)
        {
            banCo.veBanCo(g);
        }
        public void khoiTaoMangOCo()
        {
            for(int i=0; i<banCo.SoDong; i++)
            {
                for(int j=0; j<banCo.SoCot; j++)
                {
                    //khởi tạo vùng nhớ
                    arrOCo[i, j] = new OCo(i, j, new Point(j*OCo.chieuRong, i*OCo.chieuCao), 0);
                }
            }
        }
        public bool danhCo(int MouseX, int MouseY, Graphics g)
        {
            //MouseX <=> độ dài của bàn cờ
            //loại bỏ trường hợp người chơi kích vào giữa đường kẽ vạch
            if(MouseX % OCo.chieuRong == 0 || MouseY % OCo.chieuCao == 0)
            {
                return false;
            }

            //xác định vị trí ô cờ đang đánh (ô cờ đang đánh thuộc dòng? cột?)
            int cot = MouseX / OCo.chieuRong;
            int dong = MouseY / OCo.chieuCao;

            //nếu ô đó đã đánh rồi thì không đánh lại nữa
            if(arrOCo[dong, cot].SoHuu != 0)
            {
                return false;
            }

            switch (luotDi)
            {
                case 1:
                    {
                        arrOCo[dong, cot].SoHuu = 1;
                        //đi quân cờ màu đen
                        banCo.veQuanCo(g, arrOCo[dong, cot].ViTri, sbBlack);
                        //sau khi người thứ 1 đánh xong thì tới lượt người thứ 2
                        luotDi = 2;
                        break;
                    }
                case 2:
                    {
                        arrOCo[dong, cot].SoHuu = 2;
                        //đi quân cờ màu trắng
                        banCo.veQuanCo(g, arrOCo[dong, cot].ViTri, sbWhite);
                        //sau khi người thứ 2 đánh xong thì tới lượt người thứ 1
                        luotDi = 1;
                        break;
                    }
                default:
                    {
                        MessageBox.Show("Có lỗi!");
                        break;
                    }
            }
            //sau khi undo và đánh quân cờ mới thì phải xóa danh sách quân cờ redo
            //tạo mới danh sách các quân cờ đã undo
            stkCacNuocUndo = new Stack<OCo>();

            //đưa vào danh sách các nước đi
            OCo oCo = new OCo(arrOCo[dong, cot].Dong, arrOCo[dong, cot].Cot, arrOCo[dong, cot].ViTri, arrOCo[dong, cot].SoHuu);
            stkCacNuocDaDi.Push(oCo);
            return true;
        }
        public void veLaiQuanCo(Graphics g)
        {
            foreach(OCo oco in stkCacNuocDaDi)
            {
                if(oco.SoHuu == 1)
                {
                    banCo.veQuanCo(g, oco.ViTri, sbBlack);
                }
                else if(oco.SoHuu == 2)
                {
                    banCo.veQuanCo(g, oco.ViTri, sbWhite);
                }
            }
        }
        #endregion



        #region Chế độ chơi game
        public void startPlayervsPlayer(Graphics g)
        {
            ready = true;
            stkCacNuocDaDi = new Stack<OCo>();
            stkCacNuocUndo = new Stack<OCo>();
            luotDi = rd.Next(2)+1;
            if (luotDi == 1)
            {
                MessageBox.Show("Người chơi 1 đi trước");
            }
            else 
            {
                MessageBox.Show("Người chơi 2 đi trước");
            }
            
            cheDoChoi = 1;
            khoiTaoMangOCo();
            veBanCo(g);
        }
        public void startPlayervsComputer(Graphics g)
        {
            ready = true;
            stkCacNuocDaDi = new Stack<OCo>();
            stkCacNuocUndo = new Stack<OCo>();
            //random lượt đi

            luotDi = 1;
            
            cheDoChoi = 2;
            khoiTaoMangOCo();
            veBanCo(g);
            khoiDongComputer(g);
        }
        #endregion



        #region undo&redo
        public void undo(Graphics g)
        {
            //lấy ô cờ vừa mới đánh ra khỏi stack
            if(stkCacNuocDaDi.Count != 0)
            {
                OCo oco = stkCacNuocDaDi.Pop();
                //dùng từ khóa new để tạo mới ô nhớ, để khi undo/redo sẽ không bị trùng vị trí ô nhớ
                stkCacNuocUndo.Push(new OCo(oco.Dong, oco.Cot, oco.ViTri, oco.SoHuu));
                arrOCo[oco.Dong, oco.Cot].SoHuu = 0;
                banCo.xoaQuanCo(g, oco.ViTri, sbDarkGray);
                //gán lại lượt đi sau khi undo/redo
                if(luotDi == 1)
                {
                    luotDi = 2;
                }
                else
                {
                    luotDi = 1;
                }

            }
            else
            {
                MessageBox.Show("Không còn quân cờ nào trên bàn cờ!");
            }
            //veBanCo(g);
            //veLaiQuanCo(g);
        }
        public void redo(Graphics g)
        {
            //lấy ô cờ vừa mới undo ra khỏi stack
            if (stkCacNuocUndo.Count != 0)
            {
                OCo oco = stkCacNuocUndo.Pop();
                stkCacNuocDaDi.Push(new OCo(oco.Dong, oco.Cot, oco.ViTri, oco.SoHuu));
                arrOCo[oco.Dong, oco.Cot].SoHuu = oco.SoHuu;
                banCo.veQuanCo(g, oco.ViTri, oco.SoHuu == 1 ? sbBlack : sbWhite);
                if (luotDi == 1)
                {
                    luotDi = 2;
                }
                else
                {
                    luotDi = 1;
                }
            }
            
        }
        #endregion



        #region Thông báo end game
        public void ketThucTroChoi()
        {
            switch (ketThuc)
            {
                case KETTHUC.HoaCo:
                    {
                        MessageBox.Show("Hòa cờ!");
                        break;
                    }
                case KETTHUC.Player1:
                    {
                        MessageBox.Show("Người chơi 1 thắng!");
                        break;
                    }
                case KETTHUC.Player2:
                    {
                        MessageBox.Show("Người chơi 2 thắng!");
                        break;
                    }
                case KETTHUC.Computer:
                    {
                        MessageBox.Show("Máy đã thắng bạn!");
                        break;
                    }
                case KETTHUC.You:
                    {
                        MessageBox.Show("Bạn thắng!");
                        break;
                    }
            }
            ready = false;
        }
        public void ketThucTC()
        {
            if(luotDi == 1)
            {
                MessageBox.Show("Người chơi 2 thắng!");
            }
            else
            {
                MessageBox.Show("Người chơi 1 thắng!");
            }
            ready = false;
        }
        #endregion


        #region Kiểm tra chiến thắng (mới)
        public bool kiemTraChienThangPlayervsPlayer()
        {
            if(stkCacNuocDaDi.Count == banCo.SoDong * banCo.SoCot)
            {
                ketThuc = KETTHUC.HoaCo;
                return true;
            }
            foreach(OCo oco in stkCacNuocDaDi)
            {
                if(duyetNgangPhai(oco.Dong, oco.Cot, oco.SoHuu) || duyetNgangTrai(oco.Dong, oco.Cot, oco.SoHuu)
                        || duyetDocTren(oco.Dong, oco.Cot, oco.SoHuu) || duyetDocDuoi(oco.Dong, oco.Cot, oco.SoHuu)
                        || duyetCheoXuoiTren(oco.Dong, oco.Cot, oco.SoHuu) || duyetCheoXuoiDuoi(oco.Dong, oco.Cot, oco.SoHuu)
                        || duyetCheoNguocTren(oco.Dong, oco.Cot, oco.SoHuu) || duyetCheoNguocDuoi(oco.Dong, oco.Cot, oco.SoHuu))
                {
                    ketThuc = (oco.SoHuu == 1 ? KETTHUC.Player1 : KETTHUC.Player2);
                    return true;
                }
            }
            return false;
        }
       
        public bool kiemTraChienThangPlayervsComputer()
        {
            if (stkCacNuocDaDi.Count == banCo.SoDong * banCo.SoCot)
            {
                ketThuc = KETTHUC.HoaCo;
                return true;
            }
            foreach (OCo oco in stkCacNuocDaDi)
            {
                /*if (duyetDoc(oco.Dong, oco.Cot, oco.SoHuu) || duyetNgang(oco.Dong, oco.Cot, oco.SoHuu) || duyetCheoXuoi(oco.Dong, oco.Cot, oco.SoHuu) || duyetCheoNguoc(oco.Dong, oco.Cot, oco.SoHuu))
                {
                    ketThuc = (oco.SoHuu == 1 ? KETTHUC.Computer : KETTHUC.You);
                    return true;
                }*/
              
                   if (duyetNgangPhai(oco.Dong, oco.Cot, oco.SoHuu) || duyetNgangTrai(oco.Dong, oco.Cot, oco.SoHuu)
                        || duyetDocTren(oco.Dong, oco.Cot, oco.SoHuu) || duyetDocDuoi(oco.Dong, oco.Cot, oco.SoHuu)
                        || duyetCheoXuoiTren(oco.Dong, oco.Cot, oco.SoHuu) || duyetCheoXuoiDuoi(oco.Dong, oco.Cot, oco.SoHuu)
                        || duyetCheoNguocTren(oco.Dong, oco.Cot, oco.SoHuu) || duyetCheoNguocDuoi(oco.Dong, oco.Cot, oco.SoHuu))
                   {
                        ketThuc = (oco.SoHuu == 1 ? KETTHUC.Computer : KETTHUC.You);
                        return true;
                   }
            }
            return false;
        }
        #endregion


        /*#region Duyệt chiến thắng (cũ)
        private bool duyetDoc(int currDong, int currCot, int currSoHuu)
        {
            if(currDong > banCo.SoDong - 5)
            {
                return false;
            }
            int dem;
            for(dem = 1; dem < 5; dem++)
            {
                if(arrOCo[currDong + dem, currCot].SoHuu != currSoHuu)
                {
                    return false;
                }
            }
            //nếu đủ 5 quân theo hàng dọc ở các biên trên, dưới thì vẫn thắng
            if(currDong == 0 || currDong + dem == banCo.SoDong)
            {
                return true;
            }
            //xét ở 2 đầu nếu không có quân nào chặn thì thắng
            //nếu đủ 5 quân cờ trên hàng dọc nhưng đã bị chặn 2 đầu từ trước thì vẫn không tính là thắng
            if(arrOCo[currDong - 1, currCot].SoHuu == 0 || arrOCo[currDong + dem, currCot].SoHuu == 0)
            {
                return true;
            }
            return false;
        }
        private bool duyetNgang(int currDong, int currCot, int currSoHuu)
        {
            if (currCot > banCo.SoCot - 5)
            {
                return false;
            }
            int dem;
            for (dem = 1; dem < 5; dem++)
            {
                if (arrOCo[currDong, currCot + dem].SoHuu != currSoHuu)
                {
                    return false;
                }
            }
            
            if (currCot == 0 || currCot + dem == banCo.SoCot)
            {
                return true;
            }
            
            if (arrOCo[currDong, currCot - 1].SoHuu == 0 || arrOCo[currDong, currCot + dem].SoHuu == 0)
            {
                return true;
            }
            return false;
        }
        private bool duyetCheoXuoi(int currDong, int currCot, int currSoHuu)
        {
            if (currDong > banCo.SoDong - 5 || currCot > banCo.SoCot - 5)
            {
                return false;
            }
            int dem;
            for (dem = 1; dem < 5; dem++)
            {
                if (arrOCo[currDong + dem, currCot + dem].SoHuu != currSoHuu)
                {
                    return false;
                }
            }

            if (currDong == 0 || currDong + dem == banCo.SoDong || currCot == 0 || currCot + dem == banCo.SoCot)
            {
                return true;
            }

            if (arrOCo[currDong - 1, currCot - 1].SoHuu == 0 || arrOCo[currDong + dem, currCot + dem].SoHuu == 0)
            {
                return true;
            }
            return false;
        }
        private bool duyetCheoNguoc(int currDong, int currCot, int currSoHuu)
        {
            if (currDong < 4 || currCot > banCo.SoCot - 5)
            {
                return false;
            }
            int dem;
            for (dem = 1; dem < 5; dem++)
            {
                if (arrOCo[currDong - dem, currCot + dem].SoHuu != currSoHuu)
                {
                    return false;
                }
            }

            if (currDong == 4 || currDong == banCo.SoDong - 1 || currCot == 0 || currCot + dem == banCo.SoCot)
            {
                return true;
            }

            if (arrOCo[currDong + 1, currCot - 1].SoHuu == 0 || arrOCo[currDong - dem, currCot + dem].SoHuu == 0)
            {
                return true;
            }
            return false;
        }
        #endregion*/


        #region Duyệt chiến thắng (mới)
        public bool duyetNgangPhai(int dongHT, int cotHT, int SoHuu)
        {
            if (cotHT > banCo.SoCot - 5)
                return false;
            for (int dem = 1; dem <= 4; dem++)
            {
                if (arrOCo[dongHT, cotHT + dem].SoHuu != SoHuu)
                {
                    return false;
                }
            }
            return true;
        }
        public bool duyetNgangTrai(int dongHT, int cotHT, int SoHuu)
        {
            if (cotHT < 4)
                return false;
            for (int dem = 1; dem <= 4; dem++)
            {
                if (arrOCo[dongHT, cotHT - dem].SoHuu != SoHuu)
                {
                    return false;
                }
            }
            return true;
        }
        public bool duyetDocTren(int dongHT, int cotHT, int SoHuu)
        {
            if (dongHT < 4)
                return false;
            for (int dem = 1; dem <= 4; dem++)
            {
                if (arrOCo[dongHT - dem, cotHT].SoHuu != SoHuu)
                {
                    return false;
                }
            }
            return true;
        }
        public bool duyetDocDuoi(int dongHT, int cotHT, int SoHuu)
        {
            if (dongHT > banCo.SoDong - 5)
                return false;
            for (int dem = 1; dem <= 4; dem++)
            {
                if (arrOCo[dongHT + dem, cotHT].SoHuu != SoHuu)
                {
                    return false;
                }
            }
            return true;
        }

        public bool duyetCheoXuoiTren(int dongHT, int cotHT, int SoHuu)
        {
            if (dongHT < 4 || cotHT < 4)
                return false;
            for (int dem = 1; dem <= 4; dem++)
            {
                if (arrOCo[dongHT - dem, cotHT - dem].SoHuu != SoHuu)
                {
                    return false;
                }
            }
            return true;
        }
        public bool duyetCheoXuoiDuoi(int dongHT, int cotHT, int SoHuu)
        {
            if (dongHT > banCo.SoDong - 5 || cotHT > banCo.SoCot - 5)
                return false;
            for (int dem = 1; dem <= 4; dem++)
            {
                if (arrOCo[dongHT + dem, cotHT + dem].SoHuu != SoHuu)
                {
                    return false;
                }
            }
            return true;
        }
        public bool duyetCheoNguocDuoi(int dongHT, int cotHT, int SoHuu)
        {
            if (dongHT > banCo.SoDong - 5 || cotHT < 4)
                return false;
            for (int dem = 1; dem <= 4; dem++)
            {
                if (arrOCo[dongHT + dem, cotHT - dem].SoHuu != SoHuu)
                {
                    return false;
                }
            }          
            return true;
        }
        public bool duyetCheoNguocTren(int dongHT, int cotHT, int SoHuu)
        {
            if (dongHT < 4 || cotHT > banCo.SoCot - 5)
                return false;
            for (int dem = 1; dem <= 4; dem++)
            {
                if (arrOCo[dongHT - dem, cotHT + dem].SoHuu != SoHuu)
                {
                    return false;
                }
            }          
            return true;
        }
        #endregion



        #region thuật toán minimax
        //khởi tạo 2 mảng điểm để đánh giá mức độ lợi thế cho ô cờ đang xét
        private long[] arrDiemTanCong = new long[7] { 0, 3, 24, 192, 1536, 12288, 98304 }; //3,2
        private long[] arrDiemPhongThu = new long[7] { 0, 2, 18, 162, 1458, 13122, 118098 };
        
        //private long[] arrDiemTanCong = new long[7] { 0, 4, 25, 246, 7300, 6561, 59049 }; //theo phương đang xét có 7 quân ta (trên phương đó, nếu 0 có quân nào là quân ta thì +0d tấn công, nếu có 1 quân ta thì +4d tấn công, 2 quân ta thì cộng 25d tấn công...
        //private long[] arrDiemPhongThu = new long[7] { 0, 3, 24, 243, 2197, 19773, 177957 }; //theo phương đang xét có 7 quân ta (trên phương đó, nếu 0 có quân nào là quân địch thì +0d phòng ngự, nếu có 1 quân địch thì +3d phòng ngự, 2 quân địch thì cộng 24d phòng ngự...
        public void khoiDongComputer(Graphics g)
        {
            //máy đánh
            if(luotDi == 1)
            {
                //nếu chưa có nước nào đi thì thực hiện đánh cờ
                if (stkCacNuocDaDi.Count == 0)
                {
                    //đầu tiên mặc định đánh giữa bàn cờ
                    //danhCo(banCo.SoCot / 2 * OCo.chieuRong + 1, banCo.SoDong/2 * OCo.chieuCao + 1, g);
                    danhCo(rd.Next((banCo.SoCot / 2 - 3) * OCo.chieuRong + 1, (banCo.SoCot / 2 + 3) * OCo.chieuRong + 1), rd.Next((banCo.SoDong / 2 - 3) * OCo.chieuCao, (banCo.SoDong / 2 + 3) * OCo.chieuCao), g);
                }
                else
                {
                    OCo oco = TimKiemNuocDi();
                    danhCo(oco.ViTri.X + 1, oco.ViTri.Y + 1, g);
                }
            }
            
        }
        private OCo TimKiemNuocDi()
        {
            OCo oCoResult = new OCo();
            long diemMax = 0;

            //thuật toán minimax tìm điểm cao nhất để đánh
            //tìm nước đi tốt nhất cho máy
            //điểm max có thể là điểm tấn công hoặc điểm phòng thủ tùy trường hợp
            for(int i=0; i<banCo.SoDong; i++)
            {
                for(int j=0; j<banCo.SoCot; j++)
                {
                    //nếu nước cờ chưa có ai đánh và không bị cắt tỉa thì mới xét giá trị MinMax
                    if (arrOCo[i,j].SoHuu == 0 && !catTia(arrOCo[i, j]))
                    {
                        long diemTanCong = diemTanCong_DuyetDoc(i, j) + diemTanCong_DuyetNgang(i, j) + diemTanCong_DuyetCheoXuoi(i, j) + diemTanCong_DuyetCheoNguoc(i, j);
                        long diemPhongThu = diemPhongThu_DuyetDoc(i, j) + diemPhongThu_DuyetNgang(i, j) + diemPhongThu_DuyetCheoXuoi(i, j) + diemPhongThu_DuyetCheoNguoc(i, j);
                        long diemTam = diemTanCong > diemPhongThu ? diemTanCong : diemPhongThu;
                        if(diemMax < diemTam)
                        {
                            diemMax = diemTam;
                            oCoResult = new OCo(arrOCo[i,j].Dong, arrOCo[i,j].Cot, arrOCo[i,j].ViTri, arrOCo[i,j].SoHuu);
                        }
                    }
                }
            }
            return oCoResult;
        }
        #endregion




        #region Cắt tỉa alpha-beta
        bool catTia(OCo oco)
        {
            //nếu cả 8 hướng đều không có nước cờ thì cắt tỉa
            if (catTiaNgang(oco) && catTiaDoc(oco) && catTiaCheoPhai(oco) && catTiaCheoTrai(oco))
            {
                return true;
            }
               
            //chạy đến đây thì 1 trong 8 hướng vẫn có nước cờ thì không được cắt tỉa
            return false;
        }

        bool catTiaNgang(OCo oco)
        {
            //xét 4 nước cờ kế tiếp theo 4 phương 8 hướng
            //phương ngang: trái -> phải, phải -> trái
            //phương dọc: trên -> dưới, dưới -> trên
            //duyệt bên phải
            //trái -> phải
            if (oco.Cot <= banCo.SoCot - 5)
                //duyệt 4 nước cờ kế tiếp từ trái -> phải
                for (int i = 1; i <= 4; i++)
                    if (arrOCo[oco.Dong, oco.Cot + i].SoHuu != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //duyệt bên trái
            //phải -> trái
            if (oco.Cot >= 4)
                for (int i = 1; i <= 4; i++)
                    if (arrOCo[oco.Dong, oco.Cot - i].SoHuu != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
            return true;
        }
        bool catTiaDoc(OCo oco)
        {
            //trên -> dưới
            if (oco.Dong <= banCo.SoDong - 5)
                //duyệt 4 nước cờ kế tiếp
                for (int i = 1; i <= 4; i++)
                    if (arrOCo[oco.Dong + i, oco.Cot].SoHuu != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //duyệt phía dưới -> trên
            if (oco.Dong >= 4)
                //duyệt 4 nước cờ kế tiếp
                for (int i = 1; i <= 4; i++)
                    if (arrOCo[oco.Dong - i, oco.Cot].SoHuu != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
            return true;
        }
        bool catTiaCheoPhai(OCo oco)
        {
            //xét ô cờ, vị trí ô cờ (dòng nào? cột nào?) 
            //hiện tại ô cờ đang ở dòng <= số dòng của bàn cờ (20) - 5
            //nghĩa là ô cờ đang ở dòng <=15 ==> hợp lệ -> xét tiếp
            //cột >=4 -> hợp lệ -> xét tiếp
            //tại vì ta sẽ xét từ góc phải trên -> góc trái dưới ==> dòng tăng, cột giảm
            //=> ô cờ đang xét có dòng phải <=15 và cột >=4 thì khi xét các nước cờ tiếp theo sẽ hợp lệ (còn trong bàn cờ) 

            //duyệt từ trên xuống
            //góc phải trên -> góc trái dưới
            if (oco.Dong <= banCo.SoDong - 5 && oco.Cot >= 4)
                //sau khi xét ô cờ hợp lệ, xét 4 nước cờ kế tiếp theo hướng góc phải trên -> góc trái dưới
                for (int i = 1; i <= 4; i++)
                    if (arrOCo[oco.Dong + i, oco.Cot - i].SoHuu != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //duyệt từ góc trái dưới -> góc phải trên
            //dòng giảm, cột tăng
            if (oco.Cot <= banCo.SoCot - 5 && oco.Dong >= 4)
                //xét 4 nước cờ kế tiếp theo hướng góc trái dưới -> góc phải trên
                for (int i = 1; i <= 4; i++)
                    if (arrOCo[oco.Dong - i, oco.Cot + i].SoHuu != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //nếu chạy đến đây tức duyệt 2 bên đều không có nước cờ thì cắt tỉa
            return true;
        }
        bool catTiaCheoTrai(OCo oco)
        {
            //duyệt từ trên xuống
            //góc trái trên -> góc phải dưới
            //dòng tăng, cột tăng
            if (oco.Dong <= banCo.SoDong - 5 && oco.Cot <= banCo.SoCot - 5)
                //xét 4 nước cờ kế tiếp theo hướng góc trái trên -> góc phải dưới
                for (int i = 1; i <= 4; i++)
                    if (arrOCo[oco.Dong + i, oco.Cot + i].SoHuu != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //bắt đầu duyệt từ ô cờ đang xét
            //duyệt từ góc phải dưới -> góc trái trên
            //dòng giảm, cột giảm
            if (oco.Cot >= 4 && oco.Dong >= 4)
                //xét 4 nước cờ kế tiếp theo hướng góc phải dưới -> góc trái trên
                for (int i = 1; i <= 4; i++)
                    if (arrOCo[oco.Dong - i, oco.Cot - i].SoHuu != 0)//nếu có nước cờ thì không cắt tỉa
                        return false;

            //nếu chạy đến đây tức duyệt 2 bên đều không có nước đánh thì cắt tỉa
            return true;
        }

        #endregion
        




        #region Tấn công (cũ)
        private long diemTanCong_DuyetDoc(int currDong, int currCot)
        {
            long diemTong = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
       
            
            //duyệt dọc từ trên xuống
            //duyệt 4 ô cờ tiếp theo
            //dòng tăng
            for(int dem = 1; dem < 5 && currDong + dem < banCo.SoDong; dem++)
            {
                if(arrOCo[currDong + dem, currCot].SoHuu == 1)
                {
                    soQuanTa++;
                    
                }
                else if(arrOCo[currDong + dem, currCot].SoHuu == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                {
                    break;
                }
            }

            //duyệt dọc từ dưới lên
            //dòng giảm
            for (int dem = 1; dem < 5 && currDong - dem >= 0; dem++)
            {
                if (arrOCo[currDong - dem, currCot].SoHuu == 1)
                {  
                    soQuanTa++;
                    
                }
                else if (arrOCo[currDong - dem, currCot].SoHuu == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                {
                    //gặp ô cờ chưa có nước đánh thì break
                    break;
                }
            }
            //nếu ở trên bị chặn bởi quân địch và ở dưới cũng bị chặn bởi quân địch => điểm = 0
            if(soQuanDich == 2)
            {
                return 0;
            }
        //private long[] arrDiemTanCong = new long[7] { 0, 3, 24, 192, 1536, 12288, 98304 }; //3,2
        //private long[] arrDiemPhongThu = new long[7] { 0, 2, 18, 162, 1458, 13122, 118098 }; 
        //hàm heuristic đánh giá điểm của ô cờ đang xét xem có lợi thế cho quân nào
            diemTong -= arrDiemPhongThu[soQuanDich]; //số âm càng nhỏ càng lợi thế cho quân địch
            diemTong += arrDiemTanCong[soQuanTa]; //số dương càng lớn càng lợi thế cho quân ta
            return diemTong;
        }
        private long diemTanCong_DuyetNgang(int currDong, int currCot)
        {
            long diemTong = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
       
            
            //trái -> phải
            //duyệt 4 ô cờ tiếp theo
            //cột tăng
            for (int dem = 1; dem < 5 && currCot + dem < banCo.SoCot; dem++)
            {
                if (arrOCo[currDong, currCot + dem].SoHuu == 1)
                {
                    soQuanTa++;
                    
                }
                else if (arrOCo[currDong, currCot + dem].SoHuu == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                {
                    break;
                }
            }

            //phải -> trái
            //cột giảm
            for (int dem = 1; dem < 5 && currCot - dem >= 0; dem++)
            {
                if (arrOCo[currDong, currCot - dem].SoHuu == 1)
                {
                    soQuanTa++;
                    
                }
                else if (arrOCo[currDong, currCot - dem].SoHuu == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                {
                    break;
                }
            }
            
            if (soQuanDich == 2)
            {
                return 0;
            }
            //hàm heuristic
            diemTong -= arrDiemPhongThu[soQuanDich]; 
            diemTong += arrDiemTanCong[soQuanTa];
            return diemTong;
        }
        private long diemTanCong_DuyetCheoXuoi(int currDong, int currCot)
        {
            long diemTong = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
          
        
            //dòng và cột đều tăng
            //góc trái trên -> góc phải dưới
            for (int dem = 1; dem < 5 && currCot + dem < banCo.SoCot && currDong + dem < banCo.SoDong; dem++)
            {
                if (arrOCo[currDong + dem, currCot + dem].SoHuu == 1)
                {
                    soQuanTa++;
                  
                }
                else if (arrOCo[currDong + dem, currCot + dem].SoHuu == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                {
                    break;
                }
            }

            //góc phải trên -> góc trái dưới
            //dòng tăng cột giảm
            for (int dem = 1; dem < 5 && currCot - dem >= 0 && currDong + dem < banCo.SoDong; dem++)
            {
                if (arrOCo[currDong + dem, currCot - dem].SoHuu == 1)
                {
                    soQuanTa++;
                    
                }
                else if (arrOCo[currDong + dem, currCot - dem].SoHuu == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                {
                    break;
                }
            }

            if (soQuanDich == 2)
            {
                return 0;
            }
            //hàm heuristic
            diemTong -= arrDiemPhongThu[soQuanDich]; 
            diemTong += arrDiemTanCong[soQuanTa];
            return diemTong;
        }
        private long diemTanCong_DuyetCheoNguoc(int currDong, int currCot)
        {
            long diemTong = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
         
    
            //dòng giảm cột tăng
            //góc trái dưới -> góc phải trên
            for (int dem = 1; dem < 5 && currCot + dem < banCo.SoCot && currDong - dem >= 0; dem++)
            {
                if (arrOCo[currDong - dem, currCot + dem].SoHuu == 1)
                {
                    soQuanTa++;

                }
                else if (arrOCo[currDong - dem, currCot + dem].SoHuu == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                {
                    break;
                }
            }

            //góc phải dưới -> trái trên
            //cột giảm ? dòng giảm?
            for (int dem = 1; dem < 5 && currCot + dem < banCo.SoCot && currDong + dem < banCo.SoDong; dem++)
            {
                if (arrOCo[currDong + dem, currCot + dem].SoHuu == 1)
                {
                    soQuanTa++;

                }
                else if (arrOCo[currDong + dem, currCot + dem].SoHuu == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                {
                    break;
                }
            }

            if (soQuanDich == 2)
            {
                return 0;
            }
            //hàm heuristic
            diemTong -= arrDiemPhongThu[soQuanDich]; 
            diemTong += arrDiemTanCong[soQuanTa];
            return diemTong;
        }
        #endregion



        #region Phòng thủ (cũ)
        //điểm phòng thủ là điểm đánh giá quân địch để kịp thời phòng ngự ngay
        //vd: quân địch thành quân 3 thì phải chặn ngay
        private long diemPhongThu_DuyetDoc(int currDong, int currCot)
        {
            long diemTong = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
        
            //duyệt dọc từ trên xuống
            for (int dem = 1; dem < 5 && currDong + dem < banCo.SoDong; dem++)
            {
                if (arrOCo[currDong + dem, currCot].SoHuu == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrOCo[currDong + dem, currCot].SoHuu == 2)
                {
                    soQuanDich++;
                }
                else
                {
                    break;
                }
            }

            //duyệt dọc từ dưới lên
            for (int dem = 1; dem < 5 && currDong - dem >= 0; dem++)
            {
                if (arrOCo[currDong - dem, currCot].SoHuu == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrOCo[currDong - dem, currCot].SoHuu == 2)
                {
                    soQuanDich++;
                }
                else
                {
                    break;
                }
            }
            
            //quân địch đã bị quân ta chặn 2 đầu -> ko cần tăng điểm phòng ngự 
            if (soQuanTa == 2)
            {
                return 0;
            }
            //hàm heuristic
            diemTong += arrDiemPhongThu[soQuanDich];
            return diemTong;
        }
        private long diemPhongThu_DuyetNgang(int currDong, int currCot)
        {
            long diemTong = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            //trái -> phải
            for (int dem = 1; dem < 5 && currCot + dem < banCo.SoCot; dem++)
            {
                if (arrOCo[currDong, currCot + dem].SoHuu == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrOCo[currDong, currCot + dem].SoHuu == 2)
                {
                    soQuanDich++;
                }
                else
                {
                    break;
                }
            }

            //phải -> trái
            for (int dem = 1; dem < 5 && currCot - dem >= 0; dem++)
            {
                if (arrOCo[currDong, currCot - dem].SoHuu == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrOCo[currDong, currCot - dem].SoHuu == 2)
                {
                    soQuanDich++;
                }
                else
                {
                    break;
                }
            }

            if (soQuanTa == 2)
            {
                return 0;
            }
            //hàm heuristic
            diemTong += arrDiemPhongThu[soQuanDich];
            return diemTong;
        }
        private long diemPhongThu_DuyetCheoXuoi(int currDong, int currCot)
        {
            long diemTong = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            //dòng và cột đều tăng
            //góc trái trên -> góc phải dưới
            for (int dem = 1; dem < 5 && currCot + dem < banCo.SoCot && currDong + dem < banCo.SoDong; dem++)
            {
                if (arrOCo[currDong + dem, currCot + dem].SoHuu == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrOCo[currDong + dem, currCot + dem].SoHuu == 2)
                {
                    soQuanDich++;
                }
                else
                {
                    break;
                }
            }

            //góc phải trên -> góc trái dưới
            //dòng tăng cột giảm
            for (int dem = 1; dem < 5 && currCot - dem >= 0 && currDong + dem < banCo.SoDong; dem++)
            {
                if (arrOCo[currDong + dem, currCot - dem].SoHuu == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrOCo[currDong + dem, currCot - dem].SoHuu == 2)
                {
                    soQuanDich++;
                }
                else
                {
                    break;
                }
            }

            if (soQuanTa == 2)
            {
                return 0;
            }
            //hàm heuristic
            diemTong += arrDiemPhongThu[soQuanDich];
            return diemTong;
        }
        private long diemPhongThu_DuyetCheoNguoc(int currDong, int currCot)
        {
            long diemTong = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            //dòng giảm cột tăng
            //góc trái dưới -> góc phải trên
            for (int dem = 1; dem < 5 && currCot + dem < banCo.SoCot && currDong - dem >= 0; dem++)
            {
                if (arrOCo[currDong - dem, currCot + dem].SoHuu == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrOCo[currDong - dem, currCot + dem].SoHuu == 2)
                {
                    soQuanDich++;
                }
                else
                {
                    break;
                }
            }

            //góc phải dưới -> trái trên
            //cột giảm ? dòng giảm?
            for (int dem = 1; dem < 5 && currCot + dem < banCo.SoCot && currDong + dem < banCo.SoDong; dem++)
            {
                if (arrOCo[currDong + dem, currCot + dem].SoHuu == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrOCo[currDong + dem, currCot + dem].SoHuu == 2)
                {
                    soQuanDich++;
                }
                else
                {
                    break;
                }
            }

            if (soQuanTa == 2)
            {
                return 0;
            }
            //hàm heuristic
            diemTong += arrDiemPhongThu[soQuanDich];
            return diemTong;
        }
        #endregion
      




       /*#region Tấn công (mới)
         public long diemTanCong_DuyetNgang(int dongHT, int cotHT)
         {
             long DiemTanCong = 0;
             int SoQuanTa = 0;
             int SoQuanDichPhai = 0;
             int SoQuanDichTrai = 0;
             int KhoangChienThang = 0;

             //bắt đầu duyệt từ ô cờ đang xét (ô cờ đang xét này đã hợp lệ, nghĩa là chưa có ai đánh và không bị cắt tỉa)
             //bên trái -> phải, tính toán 4 ngước cờ
             //cột tăng
             for (int dem = 1; dem < 5 && cotHT + dem < banCo.SoCot; dem++)
             {
                 //ô cờ do máy đánh
                 if (arrOCo[dongHT, cotHT + dem].SoHuu == 1)
                 {
                     //nếu có 1 quân ta thì +37 điểm vào điểm tấn công
                     //có 1 quân ta ở ô cờ kế tiếp
                     if (dem == 1)
                         DiemTanCong += 37;
                         SoQuanTa++; 
                         KhoangChienThang++;
                 }
                 //ô cờ do người đánh
                 //nếu trong 4 ô cờ kế tiếp đang xét đó có quân địch (do người đánh) => tăng quân địch
                 else if (arrOCo[dongHT, cotHT + dem].SoHuu == 2)
                 {
                     SoQuanDichPhai++;
                     break;
                 }
                 //trong 4 ô cờ kế tiếp đang xét đó chưa có ô cờ được đánh
                 else
                 {
                     KhoangChienThang++;
                 }
             }
             //bắt đầu duyệt từ ô cờ đang xét
             //bên phải -> trái, tính toán 4 nước cờ
             //cột giảm
             for (int dem = 1; dem < 5 && cotHT - dem >= 0; dem++)
             {
                 if (arrOCo[dongHT, cotHT - dem].SoHuu == 1)
                 {
                     //nếu có 1 quân ta thì +37 điểm
                     if (dem == 1)
                         DiemTanCong += 37;
                         SoQuanTa++;
                         KhoangChienThang++;
                 }
                 else if (arrOCo[dongHT, cotHT - dem].SoHuu == 2)
                 {
                     SoQuanDichTrai++;
                     break;
                 }
                 else KhoangChienThang++;
             }
             //bị chặn 2 đầu khoảng chiến thắng không đủ tạo thành 5 nước
             if (SoQuanDichPhai > 0 && SoQuanDichTrai > 0 && KhoangChienThang < 5)
                 return 0;

             DiemTanCong -= arrDiemPhongThu[SoQuanDichPhai + SoQuanDichTrai];
             DiemTanCong += arrDiemTanCong[SoQuanTa];
             return DiemTanCong;
         }

         //duyệt dọc
         public long diemTanCong_DuyetDoc(int dongHT, int cotHT)
         {
             long DiemTanCong = 0;
             int SoQuanTa = 0;
             int SoQuanDichTren = 0;
             int SoQuanDichDuoi = 0;
             int KhoangChienThang = 0;

             //bên trên
             //dòng giảm
             for (int dem = 1; dem < 5  && dongHT - dem >= 0; dem++)
             {
                 if (arrOCo[dongHT - dem, cotHT].SoHuu == 1)
                 {
                     if (dem == 1)
                         DiemTanCong += 37; 
                         SoQuanTa++;
                         KhoangChienThang++;
                 }
                 else if (arrOCo[dongHT - dem, cotHT].SoHuu == 2)
                 {
                     SoQuanDichTren++;
                     break;
                 }
                 else KhoangChienThang++;
             }
             //bên dưới
             //dòng tăng
             for (int dem = 1; dem < 5 && dongHT + dem < banCo.SoDong; dem++)
             {
                 if (arrOCo[dongHT + dem, cotHT].SoHuu == 1)
                 {
                     if (dem == 1)
                         DiemTanCong += 37;
                         SoQuanTa++;
                         KhoangChienThang++;
                 }
                 else if (arrOCo[dongHT + dem, cotHT].SoHuu == 2)
                 {
                     SoQuanDichDuoi++;
                     break;
                 }
                 else KhoangChienThang++;
             }
             //bị chặn 2 đầu khoảng chiến thắng không đủ tạo thành 5 nước
             if (SoQuanDichTren > 0 && SoQuanDichDuoi > 0 && KhoangChienThang < 5)
                 return 0;

             DiemTanCong -= arrDiemPhongThu[SoQuanDichTren + SoQuanDichDuoi];
             DiemTanCong += arrDiemTanCong[SoQuanTa];
             return DiemTanCong;
         }

         //chéo xuôi
         public long diemTanCong_DuyetCheoXuoi(int dongHT, int cotHT)
         {
             long DiemTanCong = 1;
             int SoQuanTa = 0;
             int SoQuanDichCheoTren = 0;
             int SoQuanDichCheoDuoi = 0;
             int KhoangChienThang = 0;

             //dòng và cột đều tăng
             //góc trái trên -> góc phải dưới

             for (int dem = 1; dem < 5 && cotHT + dem < banCo.SoCot && dongHT + dem < banCo.SoDong; dem++)
             {
                 if (arrOCo[dongHT + dem, cotHT + dem].SoHuu == 1)
                 {
                     if (dem == 1)
                         DiemTanCong += 37;
                         SoQuanTa++;
                         KhoangChienThang++;
                 }
                 else if (arrOCo[dongHT + dem, cotHT + dem].SoHuu == 2)
                 {
                     SoQuanDichCheoTren++;
                     break;
                 }
                 else KhoangChienThang++;
             }
             //góc phải dưới -> góc trái trên
             //dòng giảm cột giảm?
             for (int dem = 1; dem < 5 && dongHT - dem >= 0 && cotHT - dem >= 0; dem++)
             {
                 if (arrOCo[dongHT - dem, cotHT - dem].SoHuu == 1)
                 {
                     if (dem == 1)
                         DiemTanCong += 37;
                         SoQuanTa++;
                         KhoangChienThang++;
                 }
                 else if (arrOCo[dongHT - dem, cotHT - dem].SoHuu == 2)
                 {
                     SoQuanDichCheoDuoi++;
                     break;
                 }
                 else KhoangChienThang++;
             }
             //bị chặn 2 đầu khoảng chiến thắng không đủ tạo thành 5 nước
             if (SoQuanDichCheoTren > 0 && SoQuanDichCheoDuoi > 0 && KhoangChienThang < 5)
                 return 0;

             DiemTanCong -= arrDiemPhongThu[SoQuanDichCheoTren + SoQuanDichCheoDuoi];
             DiemTanCong += arrDiemTanCong[SoQuanTa];
             return DiemTanCong;
         }

         //chéo ngược
         public long diemTanCong_DuyetCheoNguoc(int dongHT, int cotHT)
         {
             long DiemTanCong = 0;
             int SoQuanTa = 0;
             int SoQuanDichCheoTren = 0;
             int SoQuanDichCheoDuoi = 0;
             int KhoangChienThang = 0;

             //dòng giảm cột tăng
             //góc trái dưới -> góc phải trên
             for (int dem = 1; dem < 5 && cotHT + dem < banCo.SoCot && dongHT - dem >= 0; dem++)
             {
                 if (arrOCo[dongHT - dem, cotHT + dem].SoHuu == 1)
                 {
                     if (dem == 1)
                         DiemTanCong += 37;

                     SoQuanTa++;
                     KhoangChienThang++;

                 }
                 else if (arrOCo[dongHT - dem, cotHT + dem].SoHuu == 2)
                 {
                     SoQuanDichCheoTren++;
                     break;
                 }
                 else KhoangChienThang++;
             }
             //góc phải trên -> trái dưới
             //cột giảm ? dòng tăng?
             for (int dem = 1; dem < 5 && cotHT - dem >= 0 && dongHT + dem < banCo.SoDong; dem++)
             {
                 if (arrOCo[dongHT + dem, cotHT - dem].SoHuu == 1)
                 {
                     if (dem == 1)
                         DiemTanCong += 37;
                         SoQuanTa++;
                         KhoangChienThang++;
                 }
                 else if (arrOCo[dongHT + dem, cotHT - dem].SoHuu == 2)
                 {
                     SoQuanDichCheoDuoi++;
                     break;
                 }
                 else KhoangChienThang++;
             }
             //bị chặn 2 đầu khoảng chiến thắng không đủ tạo thành 5 nước
             if (SoQuanDichCheoTren > 0 && SoQuanDichCheoDuoi > 0 && KhoangChienThang < 5)
                 return 0;

             DiemTanCong -= arrDiemPhongThu[SoQuanDichCheoTren + SoQuanDichCheoDuoi];
             DiemTanCong += arrDiemTanCong[SoQuanTa];
             return DiemTanCong;
         }
         #endregion*/


       /*#region phòng ngự (mới)

        //duyệt ngang
        public long diemPhongThu_DuyetNgang(int dongHT, int cotHT)
        {
            long DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangChienThangPhai = 0;
            int KhoangChienThangTrai = 0;
            bool ok = false;

            //trái -> phải
            //cột tăng
            for (int dem = 1; dem < 5 && cotHT + dem < banCo.SoCot; dem++)
            {
                if (arrOCo[dongHT, cotHT + dem].SoHuu == 2)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;
                        SoQuanDich++;
                }
                else if (arrOCo[dongHT, cotHT + dem].SoHuu == 1)
                {
                    if (dem == 4)
                        DiemPhongNgu -= 170;
                        SoQuanTaTrai++;
                        break;
                }
                else
                {
                    if (dem == 1)
                        ok = true;
                    KhoangChienThangPhai++;
                }
            }

            if (SoQuanDich == 3 && KhoangChienThangPhai == 1 && ok)
                DiemPhongNgu -= 200;

            ok = false;

            //phải -> trái
            //cột giảm
            for (int dem = 1; dem < 5 && cotHT  - dem >= 0; dem++)
            {
                if (arrOCo[dongHT, cotHT - dem].SoHuu == 2)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (arrOCo[dongHT, cotHT - dem].SoHuu == 1)
                {
                    if (dem == 4)
                        DiemPhongNgu -= 170;

                    SoQuanTaPhai++;
                    break;
                }
                else
                {
                    if (dem == 1)
                        ok = true;

                    KhoangChienThangTrai++;
                }
            }

            if (SoQuanDich == 3 && KhoangChienThangTrai == 1 && ok)
                DiemPhongNgu -= 200;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChienThangTrai + KhoangChienThangPhai + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= arrDiemTanCong[SoQuanTaPhai + SoQuanTaPhai];
            DiemPhongNgu += arrDiemPhongThu[SoQuanDich];

            return DiemPhongNgu;
        }

        //duyệt dọc
        public long diemPhongThu_DuyetDoc(int dongHT, int cotHT)
        {
            long DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangChienThangTren = 0;
            int KhoangChienThangDuoi = 0;
            bool ok = false;

            //dưới -> lên
            //dòng giảm
            for (int dem = 1; dem < 5 && dongHT - dem >= 0; dem++)
            {
                if (arrOCo[dongHT - dem, cotHT].SoHuu == 2)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;

                }
                else
                    if (arrOCo[dongHT - dem, cotHT].SoHuu == 1)
                {
                    if (dem == 4)
                        DiemPhongNgu -= 170;

                    SoQuanTaPhai++;
                    break;
                }
                else
                {
                    if (dem == 1)
                        ok = true;

                    KhoangChienThangTren++;
                }
            }

            if (SoQuanDich == 3 && KhoangChienThangTren == 1 && ok)
                DiemPhongNgu -= 200;

            ok = false;
            //trên -> xuống
            //dòng tăng
            for (int dem = 1; dem < 5 && dongHT + dem < banCo.SoDong; dem++)
            {
                //gặp quân địch
                if (arrOCo[dongHT + dem, cotHT].SoHuu == 2)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (arrOCo[dongHT + dem, cotHT].SoHuu == 1)
                {
                    if (dem == 4)
                        DiemPhongNgu -= 170;

                    SoQuanTaTrai++;
                    break;
                }
                else
                {
                    if (dem == 1)
                        ok = true;

                    KhoangChienThangDuoi++;
                }
            }

            if (SoQuanDich == 3 && KhoangChienThangDuoi == 1 && ok)
                DiemPhongNgu -= 200;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChienThangTren + KhoangChienThangDuoi + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= arrDiemTanCong[SoQuanTaTrai + SoQuanTaPhai];
            DiemPhongNgu += arrDiemPhongThu[SoQuanDich];
            return DiemPhongNgu;
        }

        //chéo xuôi
        public long diemPhongThu_DuyetCheoXuoi(int dongHT, int cotHT)
        {
            long DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangChienThangTren = 0;
            int KhoangChienThangDuoi = 0;
            bool ok = false;

            //dòng và cột đều tăng
            //góc trái trên -> góc phải dưới
            for (int dem = 1; dem < 5 && dongHT + dem < banCo.SoDong  && cotHT + dem < banCo.SoCot; dem++)
            {
                if (arrOCo[dongHT + dem, cotHT + dem].SoHuu == 2)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (arrOCo[dongHT + dem, cotHT + dem].SoHuu == 1)
                {
                    if (dem == 4)
                        DiemPhongNgu -= 170;

                    SoQuanTaPhai++;
                    break;
                }
                else
                {
                    if (dem == 1)
                        ok = true;

                    KhoangChienThangTren++;
                }
            }

            if (SoQuanDich == 3 && KhoangChienThangTren == 1 && ok)
                DiemPhongNgu -= 200;

            ok = false;
            //góc phải dưới -> góc trái trên
            //dòng giảm cột giảm
            
            for (int dem = 1; dem < 5 && dongHT  - dem >= 0 && cotHT - dem >= 0; dem++)
            {
                if (arrOCo[dongHT - dem, cotHT - dem].SoHuu == 2)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (arrOCo[dongHT - dem, cotHT - dem].SoHuu == 1)
                {
                    if (dem == 4)
                        DiemPhongNgu -= 170;

                    SoQuanTaTrai++;
                    break;
                }
                else
                {
                    if (dem == 1)
                        ok = true;

                    KhoangChienThangDuoi++;
                }
            }

            if (SoQuanDich == 3 && KhoangChienThangDuoi == 1 && ok)
                DiemPhongNgu -= 200;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChienThangTren + KhoangChienThangDuoi + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= arrDiemTanCong[SoQuanTaPhai + SoQuanTaTrai];
            DiemPhongNgu += arrDiemPhongThu[SoQuanDich];

            return DiemPhongNgu;
        }

        //chéo ngược
        public long diemPhongThu_DuyetCheoNguoc(int dongHT, int cotHT)
        {
            long DiemPhongNgu = 0;
            int SoQuanTaTrai = 0;
            int SoQuanTaPhai = 0;
            int SoQuanDich = 0;
            int KhoangChienThangTren = 0;
            int KhoangChienThangDuoi = 0;
            bool ok = false;

            //dòng giảm cột tăng
            //góc trái dưới -> góc phải trên
            for (int dem = 1; dem < 5 && dongHT - dem >= 0 && cotHT + dem < banCo.SoCot; dem++)
            {

                if (arrOCo[dongHT - dem, cotHT + dem].SoHuu == 2)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (arrOCo[dongHT - dem, cotHT + dem].SoHuu == 1)
                {
                    if (dem == 4)
                        DiemPhongNgu -= 170;

                    SoQuanTaPhai++;
                    break;
                }
                else
                {
                    if (dem == 1)
                        ok = true;

                    KhoangChienThangTren++;
                }
            }


            if (SoQuanDich == 3 && KhoangChienThangTren == 1 && ok)
                DiemPhongNgu -= 200;

            ok = false;
            //góc phải trên -> trái dưới
            //cột giảm ? dòng tăng?
            for (int dem = 1; dem < 5 && dongHT + dem < banCo.SoDong && cotHT - dem >= 0; dem++)
            {
                if (arrOCo[dongHT + dem, cotHT - dem].SoHuu == 2)
                {
                    if (dem == 1)
                        DiemPhongNgu += 9;

                    SoQuanDich++;
                }
                else
                    if (arrOCo[dongHT + dem, cotHT - dem].SoHuu == 1)
                {
                    if (dem == 4)
                        DiemPhongNgu -= 170;

                    SoQuanTaTrai++;
                    break;
                }
                else
                {
                    if (dem == 1)
                        ok = true;

                    KhoangChienThangDuoi++;
                }
            }

            if (SoQuanDich == 3 && KhoangChienThangDuoi == 1 && ok)
                DiemPhongNgu -= 200;

            if (SoQuanTaPhai > 0 && SoQuanTaTrai > 0 && (KhoangChienThangTren + KhoangChienThangDuoi + SoQuanDich) < 4)
                return 0;

            DiemPhongNgu -= arrDiemTanCong[SoQuanTaTrai + SoQuanTaPhai];
            DiemPhongNgu += arrDiemPhongThu[SoQuanDich];

            return DiemPhongNgu;
        }
        #endregion*/
        
    }
}

