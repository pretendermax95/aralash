using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

[assembly: System.Reflection.AssemblyTitle("Разработчик: Саломов Максуд З.")]
[assembly: System.Reflection.AssemblyDescription("Приложение для выбора и запуска нескольких EXE-файлов.")]
[assembly: System.Reflection.AssemblyCompany("Саломов Максуд З.")]
[assembly: System.Reflection.AssemblyProduct("Запуск EXE")]

internal static class ExeLauncher
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new LauncherForm());
    }
}

internal sealed class LauncherForm : Form
{
    private readonly ListBox pathList;
    private readonly Label statusLabel;

    internal LauncherForm()
    {
        Text = "Разработчик: Саломов Максуд З.";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(540, 330);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        Label listLabel = new Label();
        listLabel.Text = "Список программ:";
        listLabel.Location = new Point(12, 15);
        listLabel.AutoSize = true;
        Controls.Add(listLabel);

        pathList = new ListBox();
        pathList.Location = new Point(12, 40);
        pathList.Size = new Size(516, 195);
        pathList.SelectionMode = SelectionMode.MultiExtended;
        Controls.Add(pathList);

        Button browseButton = new Button();
        browseButton.Text = "Обзор...";
        browseButton.Location = new Point(12, 250);
        browseButton.Size = new Size(145, 32);
        browseButton.Click += BrowseForExecutable;
        Controls.Add(browseButton);

        Button removeButton = new Button();
        removeButton.Text = "Удалить выбранные";
        removeButton.Location = new Point(167, 250);
        removeButton.Size = new Size(150, 32);
        removeButton.Click += RemoveSelected;
        Controls.Add(removeButton);

        Button runButton = new Button();
        runButton.Text = "Запустить все";
        runButton.Location = new Point(378, 250);
        runButton.Size = new Size(150, 32);
        runButton.Click += RunExecutable;
        Controls.Add(runButton);

        statusLabel = new Label();
        statusLabel.Text = "Выберите один или несколько EXE-файлов.";
        statusLabel.Location = new Point(12, 294);
        statusLabel.Size = new Size(516, 24);
        Controls.Add(statusLabel);
    }

    private void BrowseForExecutable(object sender, EventArgs e)
    {
        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.Title = "Выберите EXE-файл";
            dialog.Filter = "Программы (*.exe)|*.exe|Все файлы (*.*)|*.*";
            dialog.CheckFileExists = true;
            dialog.Multiselect = true;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string fileName in dialog.FileNames)
                {
                    bool alreadyAdded = false;
                    foreach (string existingPath in pathList.Items)
                    {
                        if (String.Equals(existingPath, fileName, StringComparison.OrdinalIgnoreCase))
                        {
                            alreadyAdded = true;
                            break;
                        }
                    }

                    if (!alreadyAdded)
                    {
                        pathList.Items.Add(fileName);
                    }
                }

                statusLabel.Text = "Выбрано программ: " + pathList.Items.Count + ".";
            }
        }
    }

    private void RemoveSelected(object sender, EventArgs e)
    {
        while (pathList.SelectedItems.Count > 0)
        {
            pathList.Items.Remove(pathList.SelectedItems[0]);
        }

        statusLabel.Text = "Выбрано программ: " + pathList.Items.Count + ".";
    }

    private void RunExecutable(object sender, EventArgs e)
    {
        if (pathList.Items.Count == 0)
        {
            MessageBox.Show(this, "Сначала выберите один или несколько EXE-файлов.", "Файлы не выбраны",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int startedCount = 0;
        string errors = "";

        foreach (string selectedPath in pathList.Items)
        {
            if (!String.Equals(Path.GetExtension(selectedPath), ".exe",
                    StringComparison.OrdinalIgnoreCase) || !File.Exists(selectedPath))
            {
                errors += selectedPath + ": файл не найден или не является EXE-файлом.\r\n\r\n";
                continue;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = selectedPath,
                    WorkingDirectory = Path.GetDirectoryName(selectedPath),
                    UseShellExecute = true
                });
                startedCount++;
            }
            catch (Exception ex)
            {
                errors += selectedPath + ":\r\n" + ex.Message + "\r\n\r\n";
            }
        }

        statusLabel.Text = "Запущено программ: " + startedCount + " из " + pathList.Items.Count + ".";
        if (!String.IsNullOrEmpty(errors))
        {
            MessageBox.Show(this, "Не удалось запустить некоторые программы:\r\n\r\n" + errors,
                "Ошибки запуска", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
