using DoAnTTNT.entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnTTNT
{
    public partial class frmCoCaro : Form
    {
        private CaroChess caroChess;
        private Graphics grs;
        public static int COOL_DOWN_STEP = 100;
        public static int COOL_DOWN_TIME = 10000; 
        public static int COOL_DOWN_INTERVAL = 100; 
        private event EventHandler playerMarked;
        public event EventHandler PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }
        private event EventHandler endGame;
        public event EventHandler EndGame
        {
            add
            {
                endGame += value;
            }
            remove
            {
                endGame -= value;
            }
        }
        public frmCoCaro()
        {
            InitializeComponent();
            caroChess = new CaroChess();

            EndGame += FrmCoCaro_EndGame;
            PlayerMarked += FrmCoCaro_PlayerMarked;

            caroChess.khoiTaoMangOCo();

            prcbCoolDown.Step = COOL_DOWN_STEP;
            prcbCoolDown.Maximum = COOL_DOWN_TIME;
            prcbCoolDown.Value = 0;

            tmCoolDown.Interval = COOL_DOWN_INTERVAL;
            

            //vẽ nền pnBanCo
            grs = pnBanCo.CreateGraphics();
        }

        private void FrmCoCaro_PlayerMarked(object sender, EventArgs e)
        {
            tmCoolDown.Start();
            prcbCoolDown.Value = 0;
        }
        public void ketThucGame1()
        {
            tmCoolDown.Stop();
            caroChess.ketThucTC();
            prcbCoolDown.Value = 0;
        }
        public void ketThucGame2()
        {
            if (caroChess.kiemTraChienThangPlayervsPlayer())
            {
                tmCoolDown.Stop();
                caroChess.ketThucTroChoi();
                prcbCoolDown.Value = 0;
            }
        }
        
        private void FrmCoCaro_EndGame(object sender, EventArgs e)
        {
            if(caroChess.CheDoChoi == 1)
            {
                ketThucGame1();
                ketThucGame2();
            }
            
        }

        private void tmLuatChoi_Tick(object sender, EventArgs e)
        {

        }

        private void frmCoCaro_Load(object sender, EventArgs e)
        {
            lblLuatChoi.Text = "- Hai bên lần lượt đánh vào từng\n ô." +"\n"+
                "- Bên nào đạt 5 con trên 1 hàng\nngang, hàng dọc hay trên 1 đường\nchéo mà không bị chặn 2 đầu là\nngười chiến thắng."
                +"\n"+"- Nếu bàn cờ đầy mà vẫn chưa bên\nnào thắng thì hòa cờ.";
            btnNewGame.Enabled = false;
            
        }

        private void pnBanCo_Paint(object sender, PaintEventArgs e)
        {
            caroChess.veBanCo(grs);
            caroChess.veLaiQuanCo(grs);
        }

        private void pnBanCo_MouseClick(object sender, MouseEventArgs e)
        {
            if (!caroChess.sanSang)
            {
                MessageBox.Show("Bạn chưa chọn chế độ chơi!");
                return;
            }
            if(caroChess.danhCo(e.X, e.Y, grs))
            {
                if(caroChess.CheDoChoi == 1)
                {
                   
                    if(playerMarked != null)
                    {
                        playerMarked(this, new EventArgs());
                    }
                    /*if (caroChess.kiemTraChienThangPlayervsPlayer())
                    {
                        caroChess.ketThucTroChoi();
                    }*/
                } 
                else
                {
                    
                    caroChess.khoiDongComputer(grs);
                    
                    if (caroChess.kiemTraChienThangPlayervsComputer())
                    {
                        caroChess.ketThucTroChoi();
                    }   
                }
            }
            
        }

        private void playerVsPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grs.Clear(pnBanCo.BackColor);
            caroChess.startPlayervsPlayer(grs);
            btnNewGame.Enabled = true;
        }

        private void playerVsComputerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tmCoolDown.Stop();
            grs.Clear(pnBanCo.BackColor);
            caroChess.startPlayervsComputer(grs);
            prcbCoolDown.Value = 0;
            btnNewGame.Enabled = true;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //grs.Clear(pnBanCo.BackColor);
            caroChess.undo(grs);
            prcbCoolDown.Value = 0;
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            //grs.Clear(pnBanCo.BackColor);
            caroChess.undo(grs);
            prcbCoolDown.Value = 0;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult ret = MessageBox.Show("Bạn có chắc chắn thoát không?", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(ret == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ret = MessageBox.Show("Bạn có chắc chắn thoát không?", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ret == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            caroChess.redo(grs);
            prcbCoolDown.Value = 0;
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            caroChess.redo(grs);
            prcbCoolDown.Value = 0;
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            if(caroChess.CheDoChoi == 1)
            {
                tmCoolDown.Stop();
                grs.Clear(pnBanCo.BackColor);
                caroChess.startPlayervsPlayer(grs);
                prcbCoolDown.Value = 0;
            }
            else
            {
                grs.Clear(pnBanCo.BackColor);
                caroChess.startPlayervsComputer(grs);    
            }
        }

        private void tmCoolDown_Tick(object sender, EventArgs e)
        {
            prcbCoolDown.PerformStep();
            if(prcbCoolDown.Value >= prcbCoolDown.Maximum)
            {
                if(caroChess.CheDoChoi == 1)
                {
                    ketThucGame1();
                }  
            }
            else
            {
                if(caroChess.CheDoChoi == 1)
                {
                    ketThucGame2();
                }   
            }
        }
    }
}
