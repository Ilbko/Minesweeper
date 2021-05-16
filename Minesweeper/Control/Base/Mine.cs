using Minesweeper.Model;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper.Control.Base
{
    //Класс мины
    public class Mine
    {
        //Кнопка, представляющая мину
        public Button button;
        //Переменная "есть ли эта кнопка миной"
        public bool isMine = false;
        //Переменная "помечена ли эта кнопка флагом"
        public bool isFlagged = false;
        //Мины в непосредственной близости
        public int nearMines = 0;

        //Метод восстановления мины в изначальное положение
        public void Restore()
        {
            this.button.BackColor = MineModel.defaultColor;
            this.button.ForeColor = Color.Black;
            this.button.Text = string.Empty;
            this.isFlagged = false;
            this.nearMines = 0;
            this.isMine = false;
        }

        //Конструктор
        public Mine() => this.button = new Button { Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold), BackColor = MineModel.defaultColor, Size = MineModel.mineSize };    
    }
}
