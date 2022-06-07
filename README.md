# 实现DataGridView多按钮操作列

在很多WinForm过程中，经常会遇到使用DataGridView进行编辑的场景，用户希望在最后放一个操作列，里面放置两个按钮，一个增加行的按钮，一个删除行的按钮；并且第一行只有增加行的按钮，没有删除行的按钮，大概的界面如下：

![]( http://res.dayuan.tech/images/datagridview01.png )

DataGridView本身提供了DataGridViewButtonColumn列类型，但问题是只会放置一个Button在单元格里，不能满足我们的需求；通过网络搜索，有很多实现方案，最终选用了通过动态生成按钮的方案，并根据所在单元格的显示范围动态设置大小和位置。

该方案在实现过程有一些细节需要注意，否则会影响用户的使用体验，先记录如下，希望为后面有需要的朋友提供一些帮助，需要的朋友可到文章末尾获取完整源码，直接复制到自己的项目可用。

#### 一、监控DataGridView的RowsAdded、RowsRemoved事件进行按钮的动态生成和移除

首先是动态生成按钮的时机，一定要在DataGridView的RowsAdded和RowsRemoved事件中去做，否则就需要再所有用户增加行的代码处都增加动态生成按钮的代码，对于后期的代码维护很不友好；而放在RowsAdded事件和RowsRemoved事件中可以一劳永逸的解决这个问题，代码如下：

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



#### 二、使用列表按行顺序保存动态生成的按钮，用以支持按钮移除和按钮位置跟新

由于用户对于DataGridView的行操作是不可以允许，可能删除任意一行，也可能在任意一行后面插入，所以我们需要记录每个按钮对应的行索引，以便于后期更新时使用；为了将增加、删除按钮方便的存储并和对应的行进行映射，我们定义了一个ActionButtons类，将每一行对应的按钮都记录在ActionButtons实例中，并按顺序存入List表中，ActionButtons实例在List列表中的的索引号即是对应的DataGridView行号，代码如下：

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



#### 三、监控DataGridView的Scroll、SizeChanged事件进行按钮的位置更新和显隐控制

###### 	在用户操作的过程中，有几个场景会需要对按钮的位置进行设置：

  - 新增的时候对按钮位置进行初始化。
  - 垂直滚动条由无到有时，按钮的位置需要更新；反之亦然。
  - 滚动条发生滚动时，按钮的位置需要更新。
  - DataGridView列宽度发生改变时，按钮的位置需要更新。
  - DataGridView大小发生变化时，按钮的位置需要更新。

###### 同样，有几个场景会需要对按钮进行显隐状态的控制：

- 当按钮所在行不在DataGridView显示范围内时要对按钮进行隐藏。
- 当按钮所在行被删除时要对按钮进行隐藏。
- DataGridView列宽度发生改变时，按钮的位置需要更新。

每次对位置和显隐的更新都需要遍历所有按钮，并且将更新代码放到线程中执行，以避免行增加、删除的同步操作对判断产生影响，具体代码如下：

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

###### 注：在列宽度发生改变时，多调用了一个Refresh方法，用于刷新界面；是因为在拖动列改变宽度时有可能会再按钮表面留下拖动的痕迹，通过刷新可以恢复按钮的样式。

最终的效果如下图：
![]( http://res.dayuan.tech/images/datagridview02.png )


