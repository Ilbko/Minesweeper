using Minesweeper.Control.Base;
using Minesweeper.Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper.Control
{
    public class Logic
    {
        //Таймер времени игры и "ссылка" на главную форму 
        private Timer timer;
        private Form1 form1;
        //Передача "ссылки" на форму, выделение памяти под таймер и установка секундного интервала
        public Logic(Form1 form1)
        {
            this.form1 = form1;
            this.timer = new Timer();
            this.timer.Interval = 1000;
        }

        //Событие тика таймера
        private void Timer_Tick(object sender, EventArgs e)
        {
            //Каждую секунду значение лейбла таймера увеличивается на один
            (form1.Controls["panel1"] as Panel).Controls["label2"].Text = (int.Parse((form1.Controls["panel1"] as Panel).Controls["label2"].Text) + 1).ToString();

            //В сапёре при достижении отметки в 999 секунд таймер прекращает свою работу. В этом случае таймер просто отписывается от события тика
            if (int.Parse((form1.Controls["panel1"] as Panel).Controls["label2"].Text) >= 999)
                this.timer.Tick -= Timer_Tick;
        }

        //Метод избавления от кнопок на форме
        private void Disposer(Form1 form1)
        {
            //При очищении циклом кнопок почему-то за один полный цикл (не итерацию) удаляется примерно половина кнопок.
            //Поэтому перед инициализацией поля нужно проверить, очистились ли все кнопки.
            bool isButtons = true;
            while (isButtons)
            {
                isButtons = false;
                foreach (var item in form1.Controls)
                {
                    if (item is Button)
                    {
                        (item as Button).Dispose();
                        isButtons = true;
                    }
                }
            }
        }

        //Метод изменения сложностей
        internal void ChangeDifficulty(string text)
        {
            MineModel.mines = null;
            //WindowsForms любит очищать не все элементы одного типа, а только половину. 
            //Поэтому нужно очистить элементы, а потом удостовериться, очистились ли все.
            Disposer(form1);

            //В зависимости от выбранного режима сложности выделяется память под массив кнопок
            switch (text)
            {
                //mineLimit - сколько мин будет на поле.
                case "Beginner":
                    {
                        MineModel.mines = new Mine[9, 9];
                        MineModel.mineLimit = 10;
                        break;
                    }
                case "Intermediate":
                    {
                        MineModel.mines = new Mine[16, 16];
                        MineModel.mineLimit = 40;
                        break;
                    }
                case "Expert":
                    {
                        MineModel.mines = new Mine[16, 30];
                        MineModel.mineLimit = 99;
                        break;
                    }
            }

            //Возможность изменения размера окна разблокируется для подгона размера окна под размер игрового поля
            form1.MaximumSize = form1.MinimumSize = new Size(0, 0);
            //Установка размера рабочего поля окна в зависимости от количества кнопок
            //(1 - расстояние между пикселями, 5 требует для себя панель в верхней части окна с кнопкой рестарта и двумя счётчиками). 
            form1.ClientSize = new Size(1 + (MineModel.mineSize.Width) * MineModel.mines.GetLength(1), 5 + form1.Controls["menuStrip1"].Size.Height + form1.Controls["panel1"].Size.Height + 
                (MineModel.mineSize.Height) * MineModel.mines.GetLength(0));

            //Установка ширины панели по ширине рабочего поля окна
            form1.Controls["panel1"].Width = form1.ClientSize.Width;
            //Установка позиции кнопки рестарта строго посередине панели
            (form1.Controls["panel1"] as Panel).Controls["button1"].Location = new Point(form1.Controls["panel1"].Size.Width / 2 - (form1.Controls["panel1"] as Panel).Controls["button1"].Size.Width / 2, 3);
            //При изменении текста лейбла также изменяется размер лейбла. Максимум, что может отобразить лейбл (счётчик времени) - 999. 
            //Изменение размера нужно учесть и корректно расположить лейбл на правой части панели. (12 - margin)
            (form1.Controls["panel1"] as Panel).Controls["label2"].Text = "999";
            (form1.Controls["panel1"] as Panel).Controls["label2"].Location = new Point(form1.Controls["panel1"].Size.Width - (form1.Controls["panel1"] as Panel).Controls["label2"].Width - 12, 15);

            //Блокировка возможности изменить размер окна
            form1.MaximumSize = form1.MinimumSize = form1.Size;

            //Инициализация массива
            for (int i = 0; i < MineModel.mines.GetLength(0); i++)
            {
                for (int j = 0; j < MineModel.mines.GetLength(1); j++)
                {
                    //Выделение памяти под объект мины
                    MineModel.mines[i, j] = new Mine();
                    //Установка локации кнопки 
                    MineModel.mines[i, j].button.Location = new Point(1 + MineModel.mineSize.Width * j, 5 + form1.Controls["menuStrip1"].Size.Height + form1.Controls["panel1"].Size.Height +
                        MineModel.mineSize.Height * i);
                    //Подписка кнопок на событие клика
                    MineModel.mines[i, j].button.MouseDown += Button_Click;
                    //Установка тега кнопки для распознания её при клике
                    MineModel.mines[i, j].button.Tag = new Point(i, j);

                    //Добавление кнопок на форму
                    form1.Controls.Add(MineModel.mines[i, j].button);
                }
            }

            //Перезагрузка игрового поля
            this.Reset();
        }

        //Метод перезагрузки игрового поля
        internal void Reset()
        {
            //Количество мин в ширину и в высоту
            int xMines = MineModel.mines.GetLength(0);
            int yMines = MineModel.mines.GetLength(1);

            //Перезагрузка - не изменение сложности игры, поэтому не нужно пересоздавать кнопки по новой.
            //Поэтому нужно просто восстановить изначальные параметры мины
            for (int i = 0; i < MineModel.mines.GetLength(0); i++)
            {
                for (int j = 0; j < MineModel.mines.GetLength(1); j++)
                {
                    MineModel.mines[i, j].Restore();
                }
            }

            Random r = new Random();

            //Текущее количество мин на поле
            MineModel.mineAmount = 0;
            while (MineModel.mineAmount < MineModel.mineLimit)
            {
                int i = r.Next(0, MineModel.mines.GetLength(0));
                int j = r.Next(0, MineModel.mines.GetLength(1));
                //Если на этой клетке уже была создана мина, то итерация пропускается
                if (MineModel.mines[i, j].isMine)
                    continue;

                //Иначе устанавливается мина и увеличивается текущее количество мин на поле
                MineModel.mines[i, j].isMine = true;
                MineModel.mineAmount++;
            }

            //Выключение таймера
            this.timer.Enabled = false;
            //При неоднократной подписке таймера на одно и то же событие, это событие выполнится столько раз, сколько была осуществлена подписка. (секунды будут расти большими шагами)
            //Поскольку отписка происходит только в случае достигания таймером отметки в 999 секунд, а подписка происходит каждый раз при
            //изменении сложности или перезагрузки поля, то нужно перед подпиской отписаться от этого события
            this.timer.Tick -= Timer_Tick;
            this.timer.Tick += Timer_Tick;

            //Сессия была перезапущена, поэтому она не закончилась
            MineModel.isEnd = false;
            //Установка значения счётчика количества мин
            (form1.Controls["panel1"] as Panel).Controls["label1"].Text = MineModel.mineLimit.ToString();
            //Установка значения счётчика времени
            (form1.Controls["panel1"] as Panel).Controls["label2"].Text = "0";
            //Установка текста кнопки перезагрузки
            (form1.Controls["panel1"] as Panel).Controls["button1"].Text = "🙂";
        }

        //Событие нажатия на кнопку (мину)
        private void Button_Click(object sender, MouseEventArgs e)
        {
            //Если игра не была закончена (выиграна или проиграна)
            if (!MineModel.isEnd)
            {
                //Если таймер выключен, то он запускается (если был произведён первый шаг)
                if (!this.timer.Enabled)
                    this.timer.Start();

                //Определение координат кнопки в массиве для того, чтобы произвести с ней работу
                Point coords = (Point)(sender as Button).Tag;

                //Если была нажата левая кнопка мыши и кнопка не была помечена флагом
                if (e.Button == MouseButtons.Left && MineModel.mines[coords.X, coords.Y].isFlagged != true)
                    SeekMines(coords.X, coords.Y);
                //Если была нажата правая кнопка (пометка флагом)
                else if (e.Button == MouseButtons.Right)
                    MarkMines(coords.X, coords.Y);

                //Если флагов больше не осталось, то идёт проверка на победу
                if (int.Parse((form1.Controls["panel1"] as Panel).Controls["label1"].Text) == 0)
                    CheckForWin();
            }
        }

        //Метод проверки на победу
        private void CheckForWin()
        {
            bool check = true;
            for (int i = 0; i < MineModel.mines.GetLength(0); i++)
            {
                for (int j = 0; j < MineModel.mines.GetLength(1); j++)
                {
                    //Если кнопка не была "раскрыта"
                    if (MineModel.mines[i, j].button.BackColor == MineModel.defaultColor)
                    {
                        //Если есть непомеченная флагом мина ИЛИ если есть нераскрытая кнопка без мины, то победы быть не может
                        if ((MineModel.mines[i, j].isFlagged == false && MineModel.mines[i, j].isMine == true) 
                            || (MineModel.mines[i, j].button.BackColor == MineModel.defaultColor && MineModel.mines[i, j].isMine == false))
                        {
                            check = false;
                            break;
                        }
                    }
                }
            }

            //Победа
            if (check)
            {
                //Изменение текста кнопки рестарта, блокировка дальнейших действий (через переменную isEnd) и отключение таймера
                (form1.Controls["panel1"] as Panel).Controls["button1"].Text = "😎";
                MineModel.isEnd = true;

                this.timer.Enabled = false;
            }
        }

        //Метод пометки мин флагом (ПКМ)
        private void MarkMines(int xCoord, int yCoord)
        {
            //Если кнопка не была уже "раскрыта"
            if (MineModel.mines[xCoord, yCoord].button.BackColor != Color.FromName(((Colors)0).ToString()))
            {
                //Если кнопка не была помечена флагом И если ещё есть флаги в наличии
                if (!MineModel.mines[xCoord, yCoord].isFlagged && int.Parse((form1.Controls["panel1"] as Panel).Controls["label1"].Text) > 0)
                {
                    //Кнопка помечается флагом визуально и в переменной
                    MineModel.mines[xCoord, yCoord].isFlagged = true;
                    MineModel.mines[xCoord, yCoord].button.Text = "#";
                    //Уменьшение количества доступных флагов
                    (form1.Controls["panel1"] as Panel).Controls["label1"].Text = (int.Parse((form1.Controls["panel1"] as Panel).Controls["label1"].Text) - 1).ToString();
                }
                //Если кнопка уже была помечена
                else if (MineModel.mines[xCoord, yCoord].isFlagged)
                {
                    //С кнопки снимается флаг и количество доступных флагов увеличивается
                    MineModel.mines[xCoord, yCoord].isFlagged = false;
                    MineModel.mines[xCoord, yCoord].button.Text = string.Empty;
                    (form1.Controls["panel1"] as Panel).Controls["label1"].Text = (int.Parse((form1.Controls["panel1"] as Panel).Controls["label1"].Text) + 1).ToString();
                }
            }            
        }

        //Метод поиска мин (рекурсия) (ЛКМ)
        private void SeekMines(int xCoord, int yCoord)
        {
            //Если нажатая кнопка не была миной
            if (!MineModel.mines[xCoord, yCoord].isMine)
            {
                //Переменная, считающая мины в непосредственной близости
                //bool check = true;
                int count = 0;
                for (int i = xCoord - 1; i <= xCoord + 1; i++)
                {
                    for (int j = yCoord - 1; j <= yCoord + 1; j++)
                    {
                        //Кнопка, которая была нажата, не проверяется
                        if (i == xCoord && j == yCoord)
                            continue;

                        //При поиске мин определённо будет выход за пределы массива, поэтому проверка находится в блоке try
                        try
                        {
                            //Если есть мина поблизости
                            if (MineModel.mines[i, j].isMine)
                                count++;
                                //check = false;
                        }
                        catch (System.IndexOutOfRangeException) { }
                    }
                }

                //Устанавливается цвет кнопки на "раскрытый"
                MineModel.mines[xCoord, yCoord].button.BackColor = Color.FromName(((Colors)0).ToString());

                //Если мин не было найдено, то можно запускать поиск для клеток в непосредственной близости
                if (count == 0)
                {
                    for (int i = xCoord - 1; i <= xCoord + 1; i++)
                    {
                        for (int j = yCoord - 1; j <= yCoord + 1; j++)
                        {
                            try
                            {
                                //Если кнопка не была раскрыта И если кнопка не была помечена флагом
                                if (MineModel.mines[i, j].button.BackColor == MineModel.defaultColor && MineModel.mines[i, j].isFlagged == false)
                                    //рекурсия
                                    SeekMines(i, j);
                            }
                            catch (System.IndexOutOfRangeException) { }
                        }
                    }
                }
                //Если были найдены мины, то на клетке будет написано количество мин поблизости в определённом цвете
                else
                {
                     MineModel.mines[xCoord, yCoord].button.ForeColor = Color.FromName(((Colors)count).ToString());
                     MineModel.mines[xCoord, yCoord].button.Text = count.ToString();
                }
            }
            //Если нажатая кнопка была миной
            else
            {
                //Игра прекращается, таймер останавливается, изменяется текст кнопки перезапуска
                MineModel.isEnd = true;
                this.timer.Enabled = false;
                (form1.Controls["panel1"] as Panel).Controls["button1"].Text = "💀";

                for (int i = 0; i < MineModel.mines.GetLength(0); i++)
                {
                    for (int j = 0; j < MineModel.mines.GetLength(1); j++)
                    {
                        //Если кнопка была миной
                        if (MineModel.mines[i, j].isMine)
                        {
                            //И если она не была помечена, то на ней отображается, что эта кнопка была миной
                            if (!MineModel.mines[i, j].isFlagged)
                                MineModel.mines[i, j].button.Text = "M";
                        }
                        //Если кнопка не была миной
                        else
                        {
                            //И если она была помечена, то на ней отображается, что она была отмечена флагом неправильно
                            if (MineModel.mines[i, j].isFlagged)
                                MineModel.mines[i, j].button.Text = "X";
                        }
                    }
                }
            }
        }
    }
}
