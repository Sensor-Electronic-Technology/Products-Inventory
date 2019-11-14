using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Common.ApplicationLayer {
    public static class ProcessAsyncHelper {
        public static async Task<ProcessResult> ExecuteShellCommand(string command, string arguments, int timeout) {
            var result = new ProcessResult();

            using (var process = new Process()) {
                // If you run bash-script on Linux it is possible that ExitCode can be 255.
                // To fix it you can try to add '#!/bin/bash' header to the script.

                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                var outputBuilder = new StringBuilder();
                var outputCloseEvent = new TaskCompletionSource<bool>();

                process.OutputDataReceived += (s, e) =>
                {
                    // The output stream has been closed i.e. the process has terminated
                    if (e.Data == null) {
                        outputCloseEvent.SetResult(true);
                    } else {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                var errorBuilder = new StringBuilder();
                var errorCloseEvent = new TaskCompletionSource<bool>();

                process.ErrorDataReceived += (s, e) =>
                {
                    // The error stream has been closed i.e. the process has terminated
                    if (e.Data == null) {
                        errorCloseEvent.SetResult(true);
                    } else {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                bool isStarted;

                try {
                    isStarted = process.Start();
                } catch (Exception error) {
                    // Usually it occurs when an executable file is not found or is not executable

                    result.Completed = true;
                    result.ExitCode = -1;
                    result.Output = error.Message;

                    isStarted = false;
                }

                if (isStarted) {
                    // Reads the output stream first and then waits because deadlocks are possible
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Creates task to wait for process exit using timeout
                    var waitForExit = WaitForExitAsync(process, timeout);

                    // Create task to wait for process exit and closing all output streams
                    var processTask = Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);

                    // Waits process completion and then checks it was not completed by timeout
                    if (await Task.WhenAny(Task.Delay(timeout), processTask) == processTask && waitForExit.Result) {
                        result.Completed = true;
                        result.ExitCode = process.ExitCode;

                        // Adds process output if it was completed with error
                        if (process.ExitCode != 0) {
                            result.Output = $"{outputBuilder}{errorBuilder}";
                        }
                    } else {
                        try {
                            // Kill hung process
                            process.Kill();
                        } catch {
                        }
                    }
                }
            }

            return result;
        }


        private static Task<bool> WaitForExitAsync(Process process, int timeout) {
            return Task.Run(() => process.WaitForExit(timeout));
        }


        public struct ProcessResult {
            public bool Completed;
            public int? ExitCode;
            public string Output;
        }
    }

    public sealed class ProcessEx : IDisposable {
        private readonly Process process;
        private readonly TaskCompletionSource<bool> exited;

        private ProcessEx(Process process) {
            this.process = process;
            this.exited = new TaskCompletionSource<bool>();
            this.process.EnableRaisingEvents = true;
            this.process.Exited += this.OnProcessExited;
            if (this.process.HasExited) {
                this.exited.TrySetResult(false);
            }
        }

        public Process Inner {
            get { return this.process; }
        }

        public Task WaitForExitAsync() {
            return this.exited.Task;
        }

        private void OnProcessExited(object sender, EventArgs e) {
            this.exited.TrySetResult(false);
        }

        public static Task<ProcessEx> StartAsync(ProcessStartInfo psi) {
            return Task.Factory.StartNew(i => new ProcessEx(Process.Start((ProcessStartInfo)i)), psi);
        }


        public async Task<bool> WaitForExitAsync(TimeSpan timeout) {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            using (CancellationTokenSource cts = new CancellationTokenSource()) {
                Task exitedTask = this.exited.Task;
                Task completedTask;
                using (cts.Token.Register(o => ((TaskCompletionSource<bool>)o).SetResult(false), tcs)) {
                    cts.CancelAfter(timeout);
                    completedTask = await Task.WhenAny(tcs.Task, exitedTask);
                }

                bool result = false;
                if (completedTask == exitedTask) {
                    await exitedTask;
                    result = true;
                }

                return result;
            }
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                this.process.EnableRaisingEvents = false;
                this.process.Exited -= this.OnProcessExited;
                this.exited.TrySetException(new ObjectDisposedException("ProcessEx"));
                this.process.Dispose();
            }
        }
    }
}
