# ʵ��DataGridView�ఴť������

?		�ںܶ�WinForm�����У�����������ʹ��DataGridView���б༭�ĳ������û�ϣ��������һ�������У��������������ť��һ�������еİ�ť��һ��ɾ���еİ�ť�����ҵ�һ��ֻ�������еİ�ť��û��ɾ���еİ�ť����ŵĽ������£�

![]( http://res.dayuan.tech/images/datagridview01.png )

?		DataGridView�����ṩ��DataGridViewButtonColumn�����ͣ���������ֻ�����һ��Button�ڵ�Ԫ��������������ǵ�����ͨ�������������кܶ�ʵ�ַ���������ѡ����ͨ����̬���ɰ�ť�ķ��������������ڵ�Ԫ�����ʾ��Χ��̬���ô�С��λ�á�

?		�÷�����ʵ�ֹ�����һЩϸ����Ҫע�⣬�����Ӱ���û���ʹ�����飬�ȼ�¼���£�ϣ��Ϊ��������Ҫ�������ṩһЩ��������Ҫ�����ѿɵ�����ĩβ��ȡ����Դ�룬ֱ�Ӹ��Ƶ��Լ�����Ŀ���á�

#### һ�����DataGridView��RowsAdded��RowsRemoved�¼����а�ť�Ķ�̬���ɺ��Ƴ�

?		�����Ƕ�̬���ɰ�ť��ʱ����һ��Ҫ��DataGridView��RowsAdded��RowsRemoved�¼���ȥ�����������Ҫ�������û������еĴ��봦�����Ӷ�̬���ɰ�ť�Ĵ��룬���ں��ڵĴ���ά���ܲ��Ѻã�������RowsAdded�¼���RowsRemoved�¼��п���һ�����ݵĽ��������⣬�������£�

```c#
private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
{
	Button btnAdd = new Button();
	btnAdd.Text = "+";
	btnAdd.Click += onAddButtonClick;
	dataGridView1.Controls.Add(btnAdd);

	Button btnRemove = new Button();
	btnRemove.Text = "-";
	btnRemove.Click += onRemoveButtonClick;
	dataGridView1.Controls.Add(btnRemove);

	...
}

private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
{
    int rowIndex = e.RowIndex;
	if (rowIndex >= 0 && rowIndex < mButtons.Count)
	{
		ActionButtons ab = mButtons[rowIndex];
		ab.AddButton.Visible = false;
		ab.AddButton.Dispose();
		ab.RemoveButton.Visible = false;
		ab.RemoveButton.Dispose();

		...
	}
}
```



#### ����ʹ���б���˳�򱣴涯̬���ɵİ�ť������֧�ְ�ť�Ƴ��Ͱ�ťλ�ø���

?		�����û�����DataGridView���в����ǲ�������������ɾ������һ�У�Ҳ����������һ�к�����룬����������Ҫ��¼ÿ����ť��Ӧ�����������Ա��ں��ڸ���ʱʹ�ã�Ϊ�˽����ӡ�ɾ����ť����Ĵ洢���Ͷ�Ӧ���н���ӳ�䣬���Ƕ�����һ��ActionButtons�࣬��ÿһ�ж�Ӧ�İ�ť����¼��ActionButtonsʵ���У�����˳�����List���У�ActionButtonsʵ����List�б��еĵ������ż��Ƕ�Ӧ��DataGridView�кţ��������£�

```c#
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

private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
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

private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
{
    int rowIndex = e.RowIndex;
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
```



#### �������DataGridView��Scroll��SizeChanged�¼����а�ť��λ�ø��º���������

###### 	���û������Ĺ����У��м�����������Ҫ�԰�ť��λ�ý������ã�

  - ������ʱ��԰�ťλ�ý��г�ʼ����
  - ��ֱ���������޵���ʱ����ť��λ����Ҫ���£���֮��Ȼ��
  - ��������������ʱ����ť��λ����Ҫ���¡�
  - DataGridView�п�ȷ����ı�ʱ����ť��λ����Ҫ���¡�
  - DataGridView��С�����仯ʱ����ť��λ����Ҫ���¡�

###### ͬ�����м�����������Ҫ�԰�ť��������״̬�Ŀ��ƣ�

- ����ť�����в���DataGridView��ʾ��Χ��ʱҪ�԰�ť�������ء�
- ����ť�����б�ɾ��ʱҪ�԰�ť�������ء�
- DataGridView�п�ȷ����ı�ʱ����ť��λ����Ҫ���¡�

?		ÿ�ζ�λ�ú������ĸ��¶���Ҫ�������а�ť�����ҽ����´���ŵ��߳���ִ�У��Ա��������ӡ�ɾ����ͬ���������жϲ���Ӱ�죬����������£�

```c#
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

         for (int i = 0; i < mButtons.Count; i++)
         {
             ActionButtons ab = mButtons[i];
             ab.AddButton.Tag = i;
             ab.RemoveButton.Tag = i;

             if (i >= firstRow && i <= lastRow)
             {
                 Rectangle rect = dataGridView1.GetCellDisplayRectangle(btnColIndex, i, false);
                 UpdateActionButtonsPosition(ab, rect, i);
             }
             else
             {
                 HideAllActionButtons(ab);
             }
         }
     });
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

```

###### ע�����п�ȷ����ı�ʱ���������һ��Refresh����������ˢ�½��棻����Ϊ���϶��иı���ʱ�п��ܻ��ٰ�ť���������϶��ĺۼ���ͨ��ˢ�¿��Իָ���ť����ʽ��

���յ�Ч������ͼ��
![]( http://res.dayuan.tech/images/datagridview02.png )

?		���Ϲ�������ʵ����Ŀ��ʹ�ã�Ч������������Ҫ�����ѿ�ͨ���·����ӻ�ȡ�������룬�����е����ݸ��Ƶ��Լ�����Ŀ�м��ɡ�

[����Դ��](https://github.com/softwaiter/datagridview)