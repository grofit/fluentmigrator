#region License
// 
// Copyright (c) 2007-2009, Sean Chambers <schambers80@gmail.com>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System.IO;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace FluentMigrator.NAnt
{
	[TaskName("migrate")]
	public class MigrateTask : Task
	{
		[TaskAttribute("database")]
		public string Database { get; set; }

		[TaskAttribute("connection")]
		public string Connection { get; set; }

		[TaskAttribute("target")]
		public string Target { get; set; }

		[TaskAttribute("namespace")]
		public string Namespace { get; set; }

		[TaskAttribute("task")]
		public string Task { get; set; }

		[TaskAttribute("to")]
		public long Version { get; set; }

		[TaskAttribute("steps")]
		public int Steps { get; set; }

		[TaskAttribute("workingdirectory")]
		public string WorkingDirectory { get; set; }

		[TaskAttribute("profile")]
		public string Profile { get; set; }

		[TaskAttribute( "timeout" )]
		public int Timeout { get; set; }

        [TaskAttribute("preview")]
        public bool Preview { get; set; }

        [TaskAttribute("outputfile")]
        public string OutputFile { get; set; }

        [TaskAttribute("migrationdirectory")]
        public string MigrationDirectory { get; set; }

        protected override void ExecuteTask()
        {
            if (string.IsNullOrEmpty(OutputFile))
            {
                var announcer = GetConsoleAnnouncer();
                var runner = GenerateRunnerContext(announcer);
                new TaskExecutor(runner).Execute();
            }
            else
            {
                using (var streamWriter = new StreamWriter(OutputFile))
                {
                    var announcer = GetOutputFileAnnouncer(streamWriter);
                    var runner = GenerateRunnerContext(announcer);
                    new TaskExecutor(runner).Execute();
                }
            }
        }

        protected IAnnouncer GetOutputFileAnnouncer(StreamWriter streamWriter)
        {
            var fileAnnouncer = new TextWriterAnnouncer(streamWriter)
            {
                ShowElapsedTime = false,
                ShowSql = true
            };

            // Not quite sure why this is needed, but it is in their console runner example
            var proxyAnnouncer = fileAnnouncer;

            var consoleAnnouncer = GetConsoleAnnouncer();

            return new CompositeAnnouncer(new[] { consoleAnnouncer, proxyAnnouncer });
        }

        protected IAnnouncer GetConsoleAnnouncer()
        {
            var consoleAnnouncer = new TextWriterAnnouncer(System.Console.Out);
            consoleAnnouncer.ShowElapsedTime = this.Verbose;
            consoleAnnouncer.ShowSql = this.Verbose;
            return consoleAnnouncer;
        }

        protected IRunnerContext GenerateRunnerContext(IAnnouncer announcer)
        {
            var runnerContext = new RunnerContext(announcer)
            {
                Database = this.Database,
                Connection = this.Connection,
                Target = this.Target,
                PreviewOnly = this.Preview,
                Namespace = this.Namespace,
                Task = this.Task,
                Version = this.Version,
                Steps = this.Steps,
                WorkingDirectory = this.WorkingDirectory,
                Profile = this.Profile,
                MigrationDirectory = this.MigrationDirectory
            };
            return runnerContext;
        }
	}
}
