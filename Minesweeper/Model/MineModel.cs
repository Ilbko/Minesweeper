using Minesweeper.Control.Base;
using System.Drawing;

namespace Minesweeper.Model
{
    //Модель игры
    public static class MineModel
    {
        //Массив мин
        public static Mine[,] mines;
        //Размер кнопки
        public static readonly Size mineSize = new Size(20, 20);
        //Максимальное количество мин и текущее количество мин
        public static int mineLimit = 0;
        public static int mineAmount = 0;
        //Цвет "нераскрытой" клетки
        public static readonly Color defaultColor = Color.PaleGoldenrod;
        //Переменная "конец ли игры" (была ли игра проиграна или выиграна)
        public static bool isEnd = false;
    }

    //Перечисление цветов, применяемое при отображении количества ближайших мин
    enum Colors
    {
        LightGray = 0, Blue, Green, Red, Purple, Maroon, Turquoise, Black, DarkGray
    }
}
