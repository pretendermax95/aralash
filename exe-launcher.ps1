Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

[System.Windows.Forms.Application]::EnableVisualStyles()

$form = New-Object System.Windows.Forms.Form
$form.Text = 'Разработчик: Саломов Максуд З.'
$form.StartPosition = 'CenterScreen'
$form.ClientSize = New-Object System.Drawing.Size(540, 330)
$form.FormBorderStyle = 'FixedDialog'
$form.MaximizeBox = $false

$listLabel = New-Object System.Windows.Forms.Label
$listLabel.Text = 'Список программ:'
$listLabel.Location = New-Object System.Drawing.Point(12, 15)
$listLabel.AutoSize = $true
$form.Controls.Add($listLabel)

$pathList = New-Object System.Windows.Forms.ListBox
$pathList.Location = New-Object System.Drawing.Point(12, 40)
$pathList.Size = New-Object System.Drawing.Size(516, 195)
$pathList.SelectionMode = [System.Windows.Forms.SelectionMode]::MultiExtended
$form.Controls.Add($pathList)

$browseButton = New-Object System.Windows.Forms.Button
$browseButton.Text = 'Обзор...'
$browseButton.Location = New-Object System.Drawing.Point(12, 250)
$browseButton.Size = New-Object System.Drawing.Size(145, 32)
$form.Controls.Add($browseButton)

$removeButton = New-Object System.Windows.Forms.Button
$removeButton.Text = 'Удалить выбранные'
$removeButton.Location = New-Object System.Drawing.Point(167, 250)
$removeButton.Size = New-Object System.Drawing.Size(150, 32)
$form.Controls.Add($removeButton)

$runButton = New-Object System.Windows.Forms.Button
$runButton.Text = 'Запустить все'
$runButton.Location = New-Object System.Drawing.Point(378, 250)
$runButton.Size = New-Object System.Drawing.Size(150, 32)
$form.Controls.Add($runButton)

$statusLabel = New-Object System.Windows.Forms.Label
$statusLabel.Text = 'Выберите один или несколько EXE-файлов.'
$statusLabel.Location = New-Object System.Drawing.Point(12, 294)
$statusLabel.Size = New-Object System.Drawing.Size(516, 24)
$form.Controls.Add($statusLabel)

$dialog = New-Object System.Windows.Forms.OpenFileDialog
$dialog.Title = 'Выберите EXE-файлы'
$dialog.Filter = 'Программы (*.exe)|*.exe|Все файлы (*.*)|*.*'
$dialog.CheckFileExists = $true
$dialog.Multiselect = $true

$browseButton.Add_Click({
    if ($dialog.ShowDialog($form) -eq [System.Windows.Forms.DialogResult]::OK) {
        foreach ($fileName in $dialog.FileNames) {
            $alreadyAdded = $false
            foreach ($existingPath in $pathList.Items) {
                if ([string]::Equals($existingPath, $fileName, [System.StringComparison]::OrdinalIgnoreCase)) {
                    $alreadyAdded = $true
                    break
                }
            }

            if (-not $alreadyAdded) {
                [void]$pathList.Items.Add($fileName)
            }
        }

        $statusLabel.Text = "Выбрано программ: $($pathList.Items.Count)."
    }
})

$removeButton.Add_Click({
    while ($pathList.SelectedItems.Count -gt 0) {
        $pathList.Items.Remove($pathList.SelectedItems[0])
    }

    $statusLabel.Text = "Выбрано программ: $($pathList.Items.Count)."
})

$runButton.Add_Click({
    if ($pathList.Items.Count -eq 0) {
        [System.Windows.Forms.MessageBox]::Show(
            $form,
            'Сначала выберите один или несколько EXE-файлов.',
            'Файлы не выбраны',
            [System.Windows.Forms.MessageBoxButtons]::OK,
            [System.Windows.Forms.MessageBoxIcon]::Warning
        ) | Out-Null
        return
    }

    $startedCount = 0
    $errors = [System.Collections.Generic.List[string]]::new()

    foreach ($selectedPath in $pathList.Items) {
        if ([System.IO.Path]::GetExtension($selectedPath) -ine '.exe' -or
            -not (Test-Path -LiteralPath $selectedPath -PathType Leaf)) {
            $errors.Add("${selectedPath}: файл не найден или не является EXE-файлом.")
            continue
        }

        try {
            Start-Process -FilePath $selectedPath `
                -WorkingDirectory (Split-Path -LiteralPath $selectedPath -Parent) `
                -ErrorAction Stop
            $startedCount++
        }
        catch {
            $errors.Add("${selectedPath}: $($_.Exception.Message)")
        }
    }

    $statusLabel.Text = "Запущено программ: $startedCount из $($pathList.Items.Count)."
    if ($errors.Count -gt 0) {
        [System.Windows.Forms.MessageBox]::Show(
            $form,
            "Не удалось запустить некоторые программы:`r`n`r`n$($errors -join "`r`n`r`n")",
            'Ошибки запуска',
            [System.Windows.Forms.MessageBoxButtons]::OK,
            [System.Windows.Forms.MessageBoxIcon]::Error
        ) | Out-Null
    }
})

[void]$form.ShowDialog()
