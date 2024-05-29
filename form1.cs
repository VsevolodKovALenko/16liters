using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LetterGame
{
    public partial class Form1 : Form
    {
        private Board board;
        private TextBox[,] textBoxes;

        public Form1()
        {
            InitializeComponent();
            board = new Board(4);
            textBoxes = new TextBox[4, 4];
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            this.Text = "Letter Game";
            this.BackColor = Color.LightBlue;
            this.Size = new Size(350, 450);

            Label instructions = new Label
            {
                Text = "Enter letters (a, b, c, d) for each cell. Leave empty for blank cells.",
                Location = new Point(10, 10),
                Size = new Size(320, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            this.Controls.Add(instructions);

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    TextBox textBox = new TextBox
                    {
                        Width = 40,
                        Height = 40,
                        MaxLength = 1,
                        Location = new Point(50 + col * 50, 50 + row * 50),
                        TextAlign = HorizontalAlignment.Center,
                        Font = new Font("Arial", 14, FontStyle.Bold),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    textBox.KeyPress += TextBox_KeyPress;
                    textBoxes[row, col] = textBox;
                    this.Controls.Add(textBox);
                }
            }

            Button solveButton = new Button
            {
                Text = "Solve",
                Location = new Point(50, 300),
                Size = new Size(75, 30),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            solveButton.Click += SolveButton_Click;
            this.Controls.Add(solveButton);

            Button resetButton = new Button
            {
                Text = "Reset",
                Location = new Point(135, 300),
                Size = new Size(75, 30),
                BackColor = Color.LightCoral,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            resetButton.Click += ResetButton_Click;
            this.Controls.Add(resetButton);

            Button checkButton = new Button
            {
                Text = "Check",
                Location = new Point(220, 300),
                Size = new Size(75, 30),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            checkButton.Click += CheckButton_Click;
            this.Controls.Add(checkButton);
            
            Button exitButton = new Button
            {
                Text = "Exit",
                Location = new Point(220, 250),
                Size = new Size(75, 30),
                BackColor = Color.LightYellow,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            exitButton.Click += ExitButton_Click;
            this.Controls.Add(exitButton);
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsAllowedCharacter(e.KeyChar);
        }

        private bool IsAllowedCharacter(char c)
        {
            return char.IsControl(c) || "abcdABCD".IndexOf(c) >= 0;
        }

        private void SolveButton_Click(object sender, EventArgs e)
        {
            if (ValidateUserInput())
            {
                for (int row = 0; row < 4; row++)
                {
                    for (int col = 0; col < 4; col++)
                    {
                        string input = textBoxes[row, col].Text.Trim().ToLower();
                        if (input.Length == 1 && "abcd".Contains(input))
                        {
                            board.SetCell(row, col, input[0]);
                        }
                        else
                        {
                            board.SetCell(row, col, '\0');
                        }
                    }
                }

                if (board.Solve())
                {
                    DisplaySolution();
                }
                else
                {
                    MessageBox.Show("No solution found.");
                }
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter letters (a, b, c, d) only.");
            }
        }

        private bool ValidateUserInput()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    string input = textBoxes[row, col].Text.Trim().ToLower();
                    if (input.Length > 0 && !"abcd".Contains(input))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void DisplaySolution()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    textBoxes[row, col].Text = board.GetCell(row, col).ToString();
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    textBoxes[row, col].Text = "";
                }
            }
        }

        private void CheckButton_Click(object sender, EventArgs e)
        {
            if (ValidateUserInput())
            {
                for (int row = 0; row < 4; row++)
                {
                    for (int col = 0; col < 4; col++)
                    {
                        string input = textBoxes[row, col].Text.Trim().ToLower();
                        if (input.Length == 1 && "abcd".Contains(input))
                        {
                            board.SetCell(row, col, input[0]);
                        }
                        else
                        {
                            board.SetCell(row, col, '\0');
                        }
                    }
                }

                if (board.IsSolutionValid())
                {
                    MessageBox.Show("The board is correctly solved!");
                }
                else
                {
                    MessageBox.Show("The board is not correctly solved.");
                }
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter letters (a, b, c, d) only.");
            }
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    class Board
    {
        private int size;
        private char[,] grid;
        private char[] letters = { 'a', 'b', 'c', 'd' };

        public Board(int size)
        {
            this.size = size;
            grid = new char[size, size];
        }

        public void SetCell(int row, int col, char letter)
        {
            grid[row, col] = letter;
        }

        public char GetCell(int row, int col)
        {
            return grid[row, col];
        }

        public bool Solve()
        {
            return Solve(0, 0);
        }

        private bool Solve(int row, int col)
        {
            if (row == size)
            {
                return true;
            }

            int nextRow = (col == size - 1) ? row + 1 : row;
            int nextCol = (col + 1) % size;

            if (grid[row, col] != '\0')
            {
                return Solve(nextRow, nextCol);
            }

            foreach (char letter in letters)
            {
                if (IsValid(row, col, letter))
                {
                    grid[row, col] = letter;
                    if (Solve(nextRow, nextCol))
                    {
                        return true;
                    }
                    grid[row, col] = '\0';
                }
            }

            return false;
        }

        private bool IsValid(int row, int col, char letter)
        {
            for (int i = 0; i < size; i++)
            {
                if (grid[row, i] == letter || grid[i, col] == letter)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSolutionValid()
        {
            for (int row = 0; row < size; row++)
            {
                if (!IsUnique(grid[row, 0], grid[row, 1], grid[row, 2], grid[row, 3]))
                {
                    return false;
                }
            }

            for (int col = 0; col < size; col++)
            {
                if (!IsUnique(grid[0, col], grid[1, col], grid[2, col], grid[3, col]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsUnique(params char[] letters)
        {
            var letterSet = new HashSet<char>(letters);
            return letterSet.Count == letters.Length && !letterSet.Contains('\0');
        }
    }
}
