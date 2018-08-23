import sys
temp = sys.path
output = ','.join(temp)

if len(sys.argv) < 2:
    sys.exit('Usage: %s path' % sys.argv[0])
print(sys.argv[1])

resultList = ['test', 'test2', 'test3']
resultDict = { 'FileName': 'thing.txt', 'Hash': '12345'}
resultDictArr = [{ 'FileName': 'thing.txt', 'Hash': '12345'}, { 'FileName': 'thing2.txt', 'Hash': '123456'}]
# output = "hello 123"
