Table of Content
0/ EasyMYP
1/ License
2/ Installation (versions 1.2 and UP)
	2.1/ First Time Installation
	2.2/ Updating the application
	2.3/ Updating the dictionary
3/ Dictionary & Filenames
	3.1/ Dictionary
	3.2/ Filenames
	3.3/ Testing filenames

A/ Notices


0/ EasyMYP
EasyMYP allows you to extract archives in the MYP format that Mythic uses for the archives of Warhammer Online. It may also be able to extract the UOP files (try at your own risk)
This application was first created in order to retrieve the LUA scripts that Mythic uses in their interface and thus be able to create our own Mods. Though, it may have not been born if Mythic had respected their promises of involving the modding community very early one.
The main contributors who participated in the creation of this project are:
 - Nicoli_s
 - Chryzo
 - Hadrian Aurelius (who fortunately cleaned up the code)

1/ License
The program is licensed under the GNU GPLv2 License that can be found here: 
 - http://www.gnu.org/licenses/old-licenses/gpl-2.0.html
 - http://www.gnu.org/licenses/old-licenses/gpl-2.0.txt

2/ Installation (versions 1.2 and UP)
	2.0/ Prerequisite
 - To make this program work you need .NET Framework 3.5. Use your prefered search engine to find the correct version for your system.
 
	2.1/ First Time Installation
To install the software for the first time follow these steps:
 - Get the latest version of EasyMYP here: http://code.google.com/p/easymyp/downloads/list
 - Get the latest version of the dictionary (Hash) here: http://code.google.com/p/easymyp/downloads/list
 - Extract the application archive where you want to install the program
 - Create a folder named Hash in the folder where you extracted the application
 - Extract the dictionary into the Hash folder

Tip & Tricks:
If you were not able to correctly create the Hash folder follow these steps:
 - Open EasyMYP
 - Open the Tool menu
 - Select merge dictionary files
 - Browse to were the hash file you have is (extracted)
 - Select it and click OK
 - The application will then create the Hash folder and put inside it the dictionary file.
 
	2.2/ Updating the application
To update the software follow these steps:
 - Get the latest version of EasyMYP here: http://code.google.com/p/easymyp/downloads/list
 - Extract the archive where you have your previous installation of the software
And that is it!

	2.3/ Updating the dictionary
To update the dictionary follow these steps:
 - Get the latest version of the dictionary here: http://code.google.com/p/easymyp/downloads/list
 - Extract the dictionary archive into the Hash folder of the application
And that is it!

3/ Dictionary & Filenames
	3.1/ Dictionary
The application uses a dictionary to be able to find the names of the files in the archives because the archives themselves do not actually store the filenames. This dictionary follows the following format:
<<ph>>#<<sh>>#<<filename>>#<<CRC>>
Where:
 - ph:			The primary hash
 - sh:			The secondary hash
 - filename:	The filename
 - CRC:			The CRC32 associated with the files, used to know if the file was updated or not (bugged at the moment)

	3.2/ Filenames
Now, the dictionary is not perfect and when you open archives you may find filenames with the following names:
12345678_90ABCDEF12345678.ext
Where:
 - First set of 8 chars:	The CRC of the file
 - _:						The character that separates the CRC from the hash
 - Second set of 8 chars:	The primary hash
 - Third set of 8 chars:	The secondary hash
 - .ext:					The estimated extension of the file, may be wrong

So, if you think you know the realname of a file you can try adding it to the dictionary manually (we advise against it) by using the format described in section 3.1. Or you can see how the application allows you to do this in the next section (3.3)

	3.3/ Testing filenames
EasyMYP provides you with different ways to test / add filenames if you want. Here is a quick list of how you can do this:
(THIS SECTION NEEDS TO BE WRITTEN)

A/ Notices
 - The authors and contributors are in no way responsible for any misdeamenour the users may do with this application. 
 - The authors and contributors can not be responsible for users corrupting their files or getting banned from the game.