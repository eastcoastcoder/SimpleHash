'''
# Created by Ethan Richardson, Nathan Knight
# written for Python3

You are required to implement in Python a very simple hash function
(that is meant more for play than for any serious production work).
Write a function that creates a 32-bit hash of a file
(represent 32-bits in hexadecimal and use it as the output for simplicity)
'''
import os
import sys
from BitVector import BitVector
from glob import glob


def main():
    if len(sys.argv) < 2:
        sys.exit('Usage: %s path' % sys.argv[0])
    if not os.path.exists(sys.argv[1]):
        sys.exit('ERROR: Path %s was not found!' % sys.argv[1])

    # Scan your directory (current), compute the hash of all your files.
    # BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    # dirScan = glob(os.path.join(BASE_DIR, 'Assets', 'input/*.*'))
    dirScan = glob(os.path.join(sys.argv[1], '*.*'))
    resultDictArr = []

    for fileName in dirScan:
        resultDict = {}
        bv = BitVector(filename=fileName)

        # Initialize the hash to all zeros
        # This bit vector will hold exactly 32 bits
        # all initialized to the 0 bit value.
        myHash = BitVector(size=32)

        # If no more data to read,
        # Stop after shifting
        while (bv.more_to_read):
            # Scan the file one byte (8bits) at a time.
            bv1 = bv.read_bits_from_file(8)

            # XOR the new byte read from the file with
            # the least significant byte (the rightmost) of the hash.
            myHash[0:8] = bv1 ^ myHash[0:8]

            # Circularly shift bit pattern in hash to left by 8 pos.
            myHash << 8

        bv.close_file_object()

        # Convert to Hex
        myHexHash = myHash.getHexStringFromBitVector()

        # Dump the hash values in some output file.
        resultDict['FileName'] = fileName
        resultDict['Hash'] = myHexHash
        resultDictArr.append(resultDict)
    print(resultDictArr)
    return resultDictArr

main()
