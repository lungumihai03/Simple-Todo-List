using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace todo
{
    public partial class Form2 : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Task Task { get; private set; }


        public Form2() : this(new Task())
        {
        }
        public Form2(Task task)
        {
            InitializeComponent();
            // Clonăm task-ul pentru a evita modificări directe
            Task = new Task
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                IsCompleted = task.IsCompleted
            };
            InitializeControls();
            PopulateFields();
        }

        private void InitializeControls()
        {
            cmbPriority.Items.AddRange(new[] { "Low", "Medium", "High" });
            if (cmbPriority.Items.Count > 0)
                cmbPriority.SelectedIndex = 0;
        }

        private void PopulateFields()
        {
            txtTitle.Text = Task.Title;
            txtDescription.Text = Task.Description;
            dtpDueDate.Value = Task.DueDate == default ? DateTime.Today : Task.DueDate;
            cmbPriority.SelectedItem = Task.Priority ?? "Low";
            Text = Task.Id == 0 ? "Add Task" : "Edit Task";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a task title.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Task.Title = txtTitle.Text.Trim();
            Task.Description = txtDescription.Text.Trim();
            Task.DueDate = dtpDueDate.Value;
            Task.Priority = cmbPriority.SelectedItem.ToString();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void dtpDueDate_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}