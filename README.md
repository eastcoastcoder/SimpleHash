# SimpleHash
Hash Function Implementation with Python
----------------------------------------
This is a very simple hash function meant more for play than actual production work. This function creates a 32-bit hexidecimal hash dump of a file through the following steps:

 - Initialize the hash to all zeros.
 - Scan the file one byte at a time.
 - Before a new byte is read from the file, circularly shift the bit pattern in the hash to the left by 8 positions.
	* Note that if there is no more data to read, you stop here (i.e., after shifting).
 - XOR the new byte read from the file with the least significant byte (the rightmost) of the hash.
 - Scan your directory (a very simple thing to do in Python), and compute the hash of all your files.
  * Reference: BitVector [https://engineering.purdue.edu/kak/dist/BitVector-3.3.2.html]()


The script itself is located under WpfApp1/Assets/Hash.py

For using the gui, you will need a copy of IronPython installed at C:\Program Files\IronPython 2.7 to get access to StdLib calls

http://ironpython.net/download/

![Screenshot](/Capture.PNG)
