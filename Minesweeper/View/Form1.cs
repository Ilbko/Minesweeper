using Minesweeper.Control;
using System;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        //Логика игры
        private Logic logic;
        public Form1()
        {
            InitializeComponent();
            //Для взаимодействия с элементами формы в класс логики единожды передаётся "ссылка" на форму
            logic = new Logic(this);
            //Подписка кнопки перезапуска игры на событие клика
            (this.Controls["panel1"] as Panel).Controls["button1"].Click += ResetButton_Click;
            //Создание поля игры уровня новичка
            logic.ChangeDifficulty("Beginner");
        }

        private void ResetButton_Click(object sender, EventArgs e) => logic.Reset();

        //Событие клика на элементы меню (выбор сложности)
        private void ToolStripMenuItem_Click(object sender, EventArgs e) =>
            logic.ChangeDifficulty((sender as ToolStripMenuItem).Text);
    }
}
