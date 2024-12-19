~~~ BUILDING ~~~

Install .NET Core SDK 8.0 or newer. From the root of this repository, run

$ dotnet build -c Release

The binaries will go to scout-dataset-code-autoupdater/bin/Release/net8.0


~~~ HOW TO USE ~~~

Create an empty directory to hold cloned repositories, e.g. ~/dataset
Go into the directory where the compiled binaries are and run

$ ./scout-dataset-code-autoupdater ~/dataset

The argument is optional. If it's not provided, the program will clone the
repositories to the current directory. For the rest of this explanation, it will
be assumed that ~/dataset has been passed to the program.
scout-substrate-dataset and scout-substrate-dataset-code will be cloned to
~/dataset. Keep these clones there, as they may be reused by the program in
future runs. An additional subdirectory ~/dataset/temp will be created to hold
temporary local clones.
Once scout-substrate-dataset-code has been cloned or pulled, it will be
duplicated inside ~/dataset/temp. This is the copy you will need to push to
Github. After the program finishes running, it will display a list of errors, if
there were any, and also it will report something like 

Now push ~/dataset/temp/scout-substrate-dataset-code.git

Go into that directory and run

$ git push --tags

to push the new data to Github. You will need to have Git credentials on this
system to be able to push. If you need to push to an intermediate remote to move
the data to a system with credentials, run

$ git remote add remote_name http://example.com/example/repo.git
$ git push --mirror remote_name

On the system you will push from, run

$ git clone http://example.com/example/repo.git example_dir
$ cd example_dir
$ git remote add Github https://github.com/CoinFabrik/scout-substrate-dataset-code.git
$ git push --tags Github
