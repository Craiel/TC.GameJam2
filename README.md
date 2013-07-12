## Command notes
How to serve repository:

	git daemon --reuseaddr --base-path=. --export-all --verbose

To clone/pull from served repo:

	git clone git://<ipaddress>/ <target>
