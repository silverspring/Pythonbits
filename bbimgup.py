#!/usr/bin/env python
'''
bbimgup - A quick and dirty images.baconbits.org uploader

Usage:

    $ bbimgup.py /path/to/image.png

If everthing has worked the image url will be printed to stdout.

'''
import json
import sys
import urllib2
import MultipartPostHandler


if __name__ == '__main__':
    fn = sys.argv[1]
    print('uploading '+fn)
    imageurl = None
    opener = urllib2.build_opener(MultipartPostHandler.MultipartPostHandler)
    try:
        params=({'ImageUp' : open(fn, "rb")})
        socket = opener.open("https://images.baconbits.org/upload.php", params)
        json_str = socket.read()
        if hasattr(json,'loads'):
            read = json.loads( json_str )
        elif hasattr(json,'read'):
            read = json.read( json_str )
        else:
            err_msg = "I cannot decipher your `json`;\n" + \
                "please report the following output to the bB forum:\n" + \
                ("%s" % dir(json))
            raise Exception( err_msg )
        imageurl = 'https://images.baconbits.org/images/' + read['ImgName']
    except Exception as e:
        print(e)
        sys.exit(1)
    print(imageurl)
    sys.exit(0)
