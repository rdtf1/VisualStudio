﻿using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using GitHub.Models;
using GitHub.Services;
using GitHub.ViewModels;
using GitHub.ViewModels.GitHubPane;
using ReactiveUI;

namespace GitHub.App.ViewModels.GitHubPane
{
    /// <inheritdoc cref="IPullRequestAnnotationItemViewModel"/>
    public class PullRequestAnnotationItemViewModel : ViewModelBase, IPullRequestAnnotationItemViewModel
    {
        readonly CheckSuiteModel checkSuite;
        readonly CheckRunModel checkRun;
        readonly IPullRequestSession session;
        readonly IPullRequestEditorService editorService;

        bool isExpanded;

        /// <summary>
        /// Initializes the <see cref="PullRequestAnnotationItemViewModel"/>.
        /// </summary>
        /// <param name="checkSuite">The check suite model.</param>
        /// <param name="checkRun">The check run model.</param>
        /// <param name="annotation">The check run annotation model.</param>
        /// <param name="session">The pull request session.</param>
        /// <param name="editorService">The pull request editor service.</param>
        public PullRequestAnnotationItemViewModel(CheckSuiteModel checkSuite, 
            CheckRunModel checkRun,
            CheckRunAnnotationModel annotation,
            IPullRequestSession session,
            IPullRequestEditorService editorService)
        {
            this.checkSuite = checkSuite;
            this.checkRun = checkRun;
            this.session = session;
            this.editorService = editorService;
            this.Annotation = annotation;

            IsFileInPullRequest = session.PullRequest.ChangedFiles.Any(model => model.FileName == annotation.Path);

            OpenAnnotation = ReactiveCommand.CreateAsyncTask(Observable.Return(IsFileInPullRequest), async x =>
            {
                await editorService.OpenDiff(session, annotation.Path, checkSuite.HeadSha, annotation.EndLine - 1);
            });
        }

        /// <inheritdoc />
        public bool IsFileInPullRequest { get; }

        /// <inheritdoc />
        public CheckRunAnnotationModel Annotation { get; }

        /// <inheritdoc />
        public string LineDescription => $"{Annotation.StartLine}:{Annotation.EndLine}";

        /// <inheritdoc />
        public ReactiveCommand<Unit> OpenAnnotation { get; }

        /// <inheritdoc />
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { this.RaiseAndSetIfChanged(ref isExpanded, value); }
        }
    }
}