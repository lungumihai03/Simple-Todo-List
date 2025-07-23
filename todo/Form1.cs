using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using static System.Windows.Forms.DataFormats;
namespace todo
{
    public partial class Form1 : Form
    {
        private List<Task> tasks = new List<Task>();
        private readonly string jsonFilePath = "tasks.json";

        public Form1()
        {
            InitializeComponent();
            // Asigurăm că SetupDataGridView este apelat primul
            SetupDataGridView();
            // Asocieri explicite ale evenimentelor
            dgvTasks.CellFormatting += dgvTasks_CellFormatting;
            dgvTasks.CellContentClick += dgvTasks_CellContentClick;
            dgvTasks.CellValueChanged += dgvTasks_CellValueChanged;
            dgvTasks.CurrentCellDirtyStateChanged += dgvTasks_CurrentCellDirtyStateChanged;
            LoadTasks();
            UpdateTaskGrid();
        }
        private void dgvTasks_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvTasks.Columns[e.ColumnIndex].Name == "Priority" && e.Value != null)
            {
                string priority = e.Value.ToString().ToLower();
                switch (priority)
                {
                    case "high":
                        e.CellStyle.BackColor = Color.Red;
                        e.CellStyle.ForeColor = Color.White;
                        break;
                    case "medium":
                        e.CellStyle.BackColor = Color.Yellow;
                        e.CellStyle.ForeColor = Color.Black;
                        break;
                    case "low":
                        e.CellStyle.BackColor = Color.LightGreen;
                        e.CellStyle.ForeColor = Color.Black;
                        break;
                    default:
                        e.CellStyle.BackColor = dgvTasks.DefaultCellStyle.BackColor;
                        e.CellStyle.ForeColor = dgvTasks.DefaultCellStyle.ForeColor;
                        break;
                }
            }
        }

        private void SetupDataGridView()
        {
            try
            {
                dgvTasks.Columns.Clear();
                dgvTasks.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Id",
                    HeaderText = "Id",
                    Visible = false
                });
                dgvTasks.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Title",
                    HeaderText = "Task Title",
                    Width = 100,
                    ReadOnly = true
                });
                dgvTasks.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Task Description",
                    Width = 250,
                    DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True },
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                   
                    ReadOnly = true
                });
                dgvTasks.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "DueDate",
                    HeaderText = "Due Date",
                    Width = 100,
                    ReadOnly = true
                });
                dgvTasks.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Priority",
                    HeaderText = "Priority",
                    Width = 80,
                    ReadOnly = true
           


                });
                dgvTasks.Columns.Add(new DataGridViewCheckBoxColumn
                {
                    Name = "IsCompleted",
                    HeaderText = "Completed",
                    Width = 80,
                    ReadOnly = false
                });
                dgvTasks.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Edit",
                    HeaderText = "Edit",
                    Text = "Edit",
                    UseColumnTextForButtonValue = true,
                    Width = 80,
                    //flat style on, and grid size 2 color yellow 
                   
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.Yellow,
                        SelectionBackColor = Color.Yellow
                    }

                });
                dgvTasks.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Delete",
                    HeaderText = "Delete",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true,
                    Width = 80,
                    //flat style on, and grid size 2 color red
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.Red,
                        SelectionBackColor = Color.Red
                    }
                });
                // Activează ajustarea automată a înălțimii rândurilor
                dgvTasks.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dgvTasks.ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up DataGridView: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTasks()
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    string json = File.ReadAllText(jsonFilePath);
                    tasks = JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveTasks()
        {
            try
            {
                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
                File.WriteAllText(jsonFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving tasks: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTaskGrid()
        {
            try
            {
                if (dgvTasks.Columns.Count == 0)
                {
                    MessageBox.Show("No columns defined in DataGridView. Cannot update grid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dgvTasks.Rows.Clear();
                foreach (var task in tasks)
                {
                    dgvTasks.Rows.Add(task.Id, task.Title, task.Description, task.DueDate.ToShortDateString(), task.Priority, task.IsCompleted);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating DataGridView: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form2 = new Form2())
            {
                if (form2.ShowDialog() == DialogResult.OK)
                {
                    var newTask = form2.Task;
                    newTask.Id = tasks.Count > 0 ? tasks[^1].Id + 1 : 1;
                    tasks.Add(newTask);
                    SaveTasks();
                    UpdateTaskGrid();
                }
            }
        }

        private void dgvTasks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= tasks.Count)
            {
                return;
            }

            var task = tasks[e.RowIndex];

            if (e.ColumnIndex == dgvTasks.Columns["Edit"].Index) // Buton Edit
            {
                using (var form2 = new Form2(task))
                {
                    if (form2.ShowDialog() == DialogResult.OK)
                    {
                        var updatedTask = form2.Task;
                        task.Title = updatedTask.Title;
                        task.Description = updatedTask.Description;
                        task.DueDate = updatedTask.DueDate;
                        task.Priority = updatedTask.Priority;
                        task.IsCompleted = updatedTask.IsCompleted;
                        SaveTasks();
                        UpdateTaskGrid();
                    }
                }
            }
            else if (e.ColumnIndex == dgvTasks.Columns["Delete"].Index) // Buton Delete
            {
                if (MessageBox.Show($"Are you sure you want to delete the task: {task.Title}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    tasks.RemoveAt(e.RowIndex);
                    SaveTasks();
                    UpdateTaskGrid();
                }
            }
        }

        private void dgvTasks_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= tasks.Count)
            {
                return;
            }

            if (e.ColumnIndex == dgvTasks.Columns["IsCompleted"].Index) // Checkbox IsCompleted
            {
                var task = tasks[e.RowIndex];
                task.IsCompleted = (bool)dgvTasks.Rows[e.RowIndex].Cells["IsCompleted"].Value;
                SaveTasks();
                UpdateTaskGrid();
            }
        }

        private void dgvTasks_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvTasks.IsCurrentCellDirty)
            {
                dgvTasks.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }

    [JsonObject]
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public bool IsCompleted { get; set; }
    }
}