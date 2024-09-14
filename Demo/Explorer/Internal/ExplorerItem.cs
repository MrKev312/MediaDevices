using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ExplorerCtrl.Internal;

[DebuggerDisplay("{Type} - {FullName}")]
internal class ExplorerItem : DependencyObject, IEquatable<ExplorerItem>
{
	public static readonly DependencyProperty NameProperty;
	public static readonly DependencyProperty FullNameProperty;
	public static readonly DependencyProperty LinkProperty;
	public static readonly DependencyProperty SizeProperty;
	public static readonly DependencyProperty DateProperty;
	public static readonly DependencyProperty TypeProperty;
	public static readonly DependencyProperty IconProperty;
	public static readonly DependencyProperty IsDirectoryProperty;
	public static readonly DependencyProperty ChildrenProperty;
	public static readonly DependencyProperty FoldersProperty;
	public static readonly DependencyProperty FilesProperty;
	public static readonly DependencyProperty FilesCountProperty;
	public static readonly DependencyProperty IsExpandedProperty;
	public static readonly DependencyProperty IsSelectedInTreeProperty;
	public static readonly DependencyProperty IsSelectedInListProperty;
	public static readonly DependencyProperty ContentProperty;

	static ExplorerItem()
	{
		NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(ExplorerItem), new FrameworkPropertyMetadata(""));
		FullNameProperty = DependencyProperty.Register("FullName", typeof(string), typeof(ExplorerItem), new FrameworkPropertyMetadata(""));
		LinkProperty = DependencyProperty.Register("Link", typeof(string), typeof(ExplorerItem), new FrameworkPropertyMetadata(""));
		SizeProperty = DependencyProperty.Register("Size", typeof(long), typeof(ExplorerItem), new FrameworkPropertyMetadata(0L));
		DateProperty = DependencyProperty.Register("Date", typeof(DateTime?), typeof(ExplorerItem), new FrameworkPropertyMetadata(null));
		TypeProperty = DependencyProperty.Register("Type", typeof(ExplorerItemType), typeof(ExplorerItem), new FrameworkPropertyMetadata(ExplorerItemType.Directory));
		IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ExplorerItem), new FrameworkPropertyMetadata(null));
		IsDirectoryProperty = DependencyProperty.Register("IsDirectory", typeof(bool), typeof(ExplorerItem), new FrameworkPropertyMetadata(true));
		ChildrenProperty = DependencyProperty.Register("Children", typeof(IEnumerable<ExplorerItem>), typeof(ExplorerItem), new FrameworkPropertyMetadata(null));
		FoldersProperty = DependencyProperty.Register("Folders", typeof(ICollectionView), typeof(ExplorerItem), new FrameworkPropertyMetadata(null));
		FilesProperty = DependencyProperty.Register("Files", typeof(ICollectionView), typeof(ExplorerItem), new FrameworkPropertyMetadata(null));
		FilesCountProperty = DependencyProperty.Register("FilesCount", typeof(int), typeof(ExplorerItem), new FrameworkPropertyMetadata(0));
		IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ExplorerItem), new FrameworkPropertyMetadata(false, OnIsExpandedChanged));
		IsSelectedInTreeProperty = DependencyProperty.Register("IsSelectedInTree", typeof(bool), typeof(ExplorerItem), new FrameworkPropertyMetadata(false, OnIsSelectedInTreeChanged));
		IsSelectedInListProperty = DependencyProperty.Register("IsSelectedInList", typeof(bool), typeof(ExplorerItem), new FrameworkPropertyMetadata(false, OnIsSelectedInListChanged));
		ContentProperty = DependencyProperty.Register("Content", typeof(IExplorerItem), typeof(ExplorerItem), new FrameworkPropertyMetadata(null));
	}

	public string Name
	{
		get { return (string)GetValue(NameProperty); }
		set { SetValue(NameProperty, value); }
	}

	public string FullName
	{
		get { return (string)GetValue(FullNameProperty); }
		set { SetValue(FullNameProperty, value); }
	}

	public ExplorerItemType Type
	{
		get { return (ExplorerItemType)GetValue(TypeProperty); }
		set { SetValue(TypeProperty, value); }
	}

	public long Size
	{
		get { return (long)GetValue(SizeProperty); }
		set { SetValue(SizeProperty, value); }
	}

	public bool IsDirectory
	{
		get { return (bool)GetValue(IsDirectoryProperty); }
		set { SetValue(IsDirectoryProperty, value); }
	}

	public IEnumerable<ExplorerItem> Children
	{
		get { return (IEnumerable<ExplorerItem>)GetValue(ChildrenProperty); }
		set { SetValue(ChildrenProperty, value); }
	}

	public ICollectionView Folders
	{
		get { return (ICollectionView)GetValue(FoldersProperty); }
		set { SetValue(FoldersProperty, value); }
	}

	public ICollectionView Files
	{
		get { return (ICollectionView)GetValue(FilesProperty); }
		set { SetValue(FilesProperty, value); }
	}

	public IExplorerItem Content
	{
		get { return (IExplorerItem)GetValue(ContentProperty); }
		set { SetValue(ContentProperty, value); }
	}

	public bool IsExpanded
	{
		get { return (bool)GetValue(IsExpandedProperty); }
		set { SetValue(IsExpandedProperty, value); }
	}

	public bool IsSelectedInTree
	{
		get { return (bool)GetValue(IsSelectedInTreeProperty); }
		set { SetValue(IsSelectedInTreeProperty, value); }
	}

	public bool IsSelectedInList
	{
		get { return (bool)GetValue(IsSelectedInListProperty); }
		set { SetValue(IsSelectedInListProperty, value); }
	}

	private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		ExplorerItem explorerItem = (ExplorerItem)d;
		bool newValue = (bool)e.NewValue;
		if (newValue)
		{
			explorerItem.Refresh(false);
		}
	}

	private static void OnIsSelectedInTreeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		ExplorerItem explorerItem = (ExplorerItem)d;
		bool newValue = (bool)e.NewValue;
		if (newValue)
		{
			explorerItem.Refresh(false);
		}
	}
	private static void OnIsSelectedInListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{

	}

	private bool isInitialFilled;
	protected ObservableCollection<ExplorerItem> children;
	private CollectionViewSource folders;
	private CollectionViewSource files;


	public ExplorerItem(IExplorerItem content, ExplorerItem parent)
	{
		Parent = parent;
		content.Refresh += OnRefresh;

		SetValue(NameProperty, content.Name);
		SetValue(FullNameProperty, content.FullName);
		SetValue(LinkProperty, content.Link);
		SetValue(SizeProperty, content.Size);
		SetValue(DateProperty, content.CreationDate);
		SetValue(TypeProperty, content.Type);
		SetValue(IconProperty, content.Icon);
		SetValue(IsDirectoryProperty, content.IsDirectory);

		Content = content;

		children = [];
		if (content.HasChildren)
		{
			children.Add(new ExplorerItem(new Dummy(), this));
		}

		folders = new CollectionViewSource() { Source = children };
		folders.View.SortDescriptions.Add(new SortDescription(nameof(Name), ListSortDirection.Ascending));
		folders.View.Filter = i => ((ExplorerItem)i).IsDirectory;
		files = new CollectionViewSource() { Source = children };
		files.View.SortDescriptions.Add(new SortDescription(nameof(IsDirectory), ListSortDirection.Descending));
		files.View.SortDescriptions.Add(new SortDescription(nameof(Name), ListSortDirection.Ascending));

		Children = children;
		Folders = folders.View;
		Files = files.View;
	}

	internal ExplorerItem Parent { get; private set; }

	public void OnRefresh(object sender, RefreshEventArgs e)
	{
		Refresh(e.Recursive);
	}

	public void Refresh(bool recursive)
	{
		if (!isInitialFilled)
		{
			children.Clear();
			Content.Children?.Select(i => new ExplorerItem(i, this)).ToList().ForEach(e => children.Add(e));
			isInitialFilled = true;
		}
		else
		{
			List<IExplorerItem> list = Content.Children?.ToList();
			List<IExplorerItem> add = list?.Where(i => !children.Select(c => c.Content).Contains(i)).ToList();
			List<ExplorerItem> del = children.Where(c => !list.Contains(c.Content)).ToList();

			add?.ForEach(i => children.Add(new ExplorerItem(i, this)));
			del?.ForEach(e => children.Remove(e));

			if (recursive)
			{
				children.Where(c => c.isInitialFilled).ToList().ForEach(c => c.Refresh(recursive));
			}
		}
	}

	#region  IEquatable<ExplorerItem>

	public virtual bool Equals(ExplorerItem other)
	{
		return other.Content == Content;
	}

	#endregion
}
