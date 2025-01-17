using UIKit;
using System;
using CodeHub.iOS.WebViews;
using System.Threading.Tasks;
using MvvmCross.Platform;
using CodeHub.Core.Services;
using CodeHub.Core.ViewModels.Gists;

namespace CodeHub.iOS.ViewControllers.Gists
{
    public class GistFileViewController : CodeHub.iOS.Views.Source.FileSourceView
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var vm = ViewModel as GistFileViewModel;
            vm.Bind(x => x.ContentPath).Subscribe(x => LoadSource(new Uri("file://" + x)));
        }

        async Task LoadSource(Uri fileUri)
        {
            var fontSize = (int)UIFont.PreferredSubheadline.PointSize;
            var content = System.IO.File.ReadAllText(fileUri.LocalPath, System.Text.Encoding.UTF8);

            if (ViewModel.IsMarkdown)
            {
                var markdownContent = await Mvx.Resolve<IApplicationService>().Client.Markdown.GetMarkdown(content);
                var model = new DescriptionModel(markdownContent, fontSize);
                var htmlContent = new MarkdownView { Model = model };
                LoadContent(htmlContent.GenerateString());
            }
            else
            {
                var model = new SourceBrowserModel(content, "idea", fontSize, fileUri.LocalPath);
                var contentView = new SyntaxHighlighterView { Model = model };
                LoadContent(contentView.GenerateString());
            }
        }
    }
}

