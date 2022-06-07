namespace DataGridOperationButtons
{
    public partial class Form1 : Form
    {
        internal class ActionButtons
        {
            public ActionButtons(Button addButton, Button removeButton)
            {
                AddButton = addButton;
                RemoveButton = removeButton;
            }

            public Button AddButton { get; set; }
            public Button RemoveButton { get; set; }
        }

        List<ActionButtons> mButtons = new List<ActionButtons>();
        int btnColIndex = 5;

        public Form1()
        {
            InitializeComponent();
        }

        private void AddButtons()
        {
            Button btnAdd = new Button();
            btnAdd.Text = "+";
            btnAdd.Click += onAddButtonClick;
            dataGridView1.Controls.Add(btnAdd);

            Button btnRemove = new Button();
            btnRemove.Text = "-";
            btnRemove.Click += onRemoveButtonClick;
            dataGridView1.Controls.Add(btnRemove);

            mButtons.Add(new ActionButtons(btnAdd, btnRemove));
        }

        private void RemoveButtons(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < mButtons.Count)
            {
                ActionButtons ab = mButtons[rowIndex];
                ab.AddButton.Visible = false;
                ab.AddButton.Dispose();
                ab.RemoveButton.Visible = false;
                ab.RemoveButton.Dispose();

                mButtons.RemoveAt(rowIndex);
            }
        }

        private void HideAllActionButtons(ActionButtons ab)
        {
            dataGridView1.BeginInvoke(() =>
            {
                ab.AddButton.Visible = ab.RemoveButton.Visible = false;
            });
        }

        private void UpdateActionButtonsPosition(ActionButtons ab, Rectangle rect, int rowIndex)
        {
            dataGridView1.BeginInvoke(() =>
            {
                ab.AddButton.Location = new Point(rect.Left + 5, rect.Top + 5);
                ab.AddButton.Size = new Size(rect.Width / 2 - 10, rect.Height - 10);
                ab.AddButton.Visible = true;

                ab.RemoveButton.Location = new Point(ab.AddButton.Left + ab.AddButton.Width + 5, rect.Top + 5);
                ab.RemoveButton.Size = ab.AddButton.Size;
                ab.RemoveButton.Visible = rowIndex > 0;
            });
        }

        private void RepositionActionButtons()
        {
            Task.Run(() =>
            {
                    Thread.Sleep(100);

                    int firstRow = dataGridView1.FirstDisplayedScrollingRowIndex;
                    int lastRow = firstRow + dataGridView1.DisplayedRowCount(false);
                    int firstCol = dataGridView1.FirstDisplayedScrollingColumnIndex;
                    int lastCol = firstCol + dataGridView1.DisplayedColumnCount(true);

                    for (int i = 0; i < mButtons.Count; i++)
                    {
                        ActionButtons ab = mButtons[i];
                        ab.AddButton.Tag = i;
                        ab.RemoveButton.Tag = i;

                        if (i >= firstRow && i <= lastRow &&
                            btnColIndex >= firstCol && btnColIndex <= lastCol)
                        {
                            Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(btnColIndex, i, false);
                            UpdateActionButtonsPosition(ab, rect, i);
                        }
                        else
                        {
                            HideAllActionButtons(ab);
                        }
                    }
            });
        }

        private void onAddButtonClick(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                Button btn = (Button)sender;
                int rowIndex = (int)btn.Tag;
                if (rowIndex == dataGridView1.Rows.Count - 1)
                {
                    dataGridView1.Rows.Add();
                }
                else
                {
                    dataGridView1.Rows.Insert(rowIndex + 1, 1);
                }
            }
        }

        private void onRemoveButtonClick(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                Button btn = (Button)sender;
                int rowIndex = (int)btn.Tag;
                dataGridView1.Rows.RemoveAt(rowIndex);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            AddButtons();
            RepositionActionButtons();
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            RemoveButtons(e.RowIndex);
            RepositionActionButtons();
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            RepositionActionButtons();
        }

        private void dataGridView1_SizeChanged(object sender, EventArgs e)
        {
            RepositionActionButtons();
        }

        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            RepositionActionButtons();
            this.Refresh();
        }
    }
}