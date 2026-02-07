using Backup.Core.Models;
using Backup.Core.Services;
using DevExpress.XtraEditors;

namespace Backup.UI;

public sealed class MainForm : XtraForm
{
    private readonly TextEdit _planName = new();
    private readonly ButtonEdit _sourcePath = new();
    private readonly ButtonEdit _destinationPath = new();
    private readonly CheckEdit _compress = new();
    private readonly SpinEdit _retention = new();
    private readonly SimpleButton _runButton = new();
    private readonly MemoEdit _output = new();

    public MainForm()
    {
        Text = "Yedekleme Programı";
        Width = 760;
        Height = 520;

        InitializeLayout();
    }

    private void InitializeLayout()
    {
        _planName.Properties.Placeholder = "Plan adı";
        _sourcePath.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        _sourcePath.Properties.Buttons[0].Caption = "Kaynak";
        _destinationPath.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        _destinationPath.Properties.Buttons[0].Caption = "Hedef";
        _compress.Text = "Zip olarak sıkıştır";
        _compress.Checked = true;
        _retention.Properties.MinValue = 1;
        _retention.Properties.MaxValue = 365;
        _retention.EditValue = 7;
        _runButton.Text = "Yedekle";
        _output.Properties.ReadOnly = true;
        _output.Properties.ScrollBars = ScrollBars.Vertical;

        _sourcePath.ButtonClick += (_, _) => _sourcePath.Text = SelectFolder(_sourcePath.Text);
        _destinationPath.ButtonClick += (_, _) => _destinationPath.Text = SelectFolder(_destinationPath.Text);
        _runButton.Click += (_, _) => RunBackup();

        var layout = new DevExpress.XtraLayout.LayoutControl { Dock = DockStyle.Fill };
        var group = new DevExpress.XtraLayout.LayoutControlGroup();
        layout.Root = group;

        layout.Controls.Add(_planName);
        layout.Controls.Add(_sourcePath);
        layout.Controls.Add(_destinationPath);
        layout.Controls.Add(_compress);
        layout.Controls.Add(_retention);
        layout.Controls.Add(_runButton);
        layout.Controls.Add(_output);

        group.AddItem("Plan Adı", _planName);
        group.AddItem("Kaynak Klasör", _sourcePath);
        group.AddItem("Hedef Klasör", _destinationPath);
        group.AddItem(string.Empty, _compress);
        group.AddItem("Saklama (gün)", _retention);
        group.AddItem(string.Empty, _runButton);
        group.AddItem("Çıktı", _output).TextVisible = true;

        Controls.Add(layout);
    }

    private void RunBackup()
    {
        var plan = new BackupPlan
        {
            Name = string.IsNullOrWhiteSpace(_planName.Text) ? "Plan" : _planName.Text,
            SourceDirectory = _sourcePath.Text,
            DestinationDirectory = _destinationPath.Text,
            CompressAsZip = _compress.Checked,
            RetentionCount = Convert.ToInt32(_retention.Value)
        };

        var service = new BackupService();
        var result = service.Run(plan);

        _output.Text = $"Başlangıç: {result.StartedAt:dd.MM.yyyy HH:mm:ss}\r\n" +
                       $"Bitiş: {result.FinishedAt:dd.MM.yyyy HH:mm:ss}\r\n" +
                       $"Dosya sayısı: {result.CopiedFiles.Count}\r\n" +
                       $"Toplam boyut: {result.TotalBytes / 1024.0:N2} KB\r\n" +
                       $"Çıktı: {result.DestinationPath}";
    }

    private static string SelectFolder(string initialPath)
    {
        using var dialog = new FolderBrowserDialog
        {
            SelectedPath = initialPath
        };

        return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : initialPath;
    }
}
