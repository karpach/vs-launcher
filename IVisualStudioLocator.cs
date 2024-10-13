using EnvDTE;

namespace Karpach.VisualStudio.Launcher;

public interface IVisualStudioLocator
{
	_DTE[] GetIDEInstances(bool openSolutionsOnly);
}