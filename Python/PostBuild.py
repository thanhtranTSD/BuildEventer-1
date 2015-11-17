# <License>
# Copyright 2015 Virtium Technology
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
    # http ://www.apache.org/licenses/LICENSE-2.0
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
# </License>

import os
import sys
import argparse

from distutils.dir_util import copy_tree
from distutils.file_util import copy_file
import xml.etree.ElementTree as ET

# Region constansts 
CONFIG_FILE_NAME         = "config.txt"
XML_ATTRIBUTE_ACTIONS    = "Actions"
XML_ATTRIBUTE_NAME       = "Name"
XML_NODE_DESTINATIONS    = "Destinations"
XML_NODE_SOURCES         = "Sources"
PATH_SEPARATOR_CHAR      = "\\"

PROGRAM_NAME             = "BuildEventer"
PARSER_PREFIX_CHAR       = '@'
CONFLICT_HANDLER_RESOLVE = "resolve"
#endregion constansts

def GetActionSources(iAction):
    paths = []
    sources = iAction.find(XML_NODE_SOURCES)
    for path in sources:
        path = path.text.replace(PATH_SEPARATOR_CHAR, "", 1)
        paths.append(path.strip())
    return paths

def GetActionDestinations(iAction):
    paths = []
    sources = iAction.find(XML_NODE_DESTINATIONS)
    for path in sources:
        path = path.text.replace(PATH_SEPARATOR_CHAR, "", 1)
        paths.append(path.strip())
    return paths

def PrintCopyTo(iSource, iDestination):
    print "{:3s}Copy {} to {}".format("", iSource, iDestination)

def PrintCopySuccess():
    print "{:3s}Action success.".format("")

def CopyFolder(iSource, iDestination, iIsOverwritten = False):
    if True == os.path.isdir(iSource):
        newDestination = os.path.join(iDestination, os.path.basename(iSource))
        PrintCopyTo(iSource, newDestination)
        copy_tree(iSource, newDestination)
        PrintCopySuccess()
    else:
        if False == os.path.exists(iDestination):
            os.makedirs(iDestination)
        PrintCopyTo(iSource, iDestination)
        newPath = os.path.join(iDestination, os.path.basename(iSource))
        if True == os.path.exists(newPath):
            if True == iIsOverwritten:
                copy_file(iSource, iDestination, update=1)
                print "{:3s}WARINING: {} has already existed in {}. File is overwritten.".format("", os.path.basename(newPath), iDestination)
            else:
                print "{:3s}WARINING: {} has already existed in {}. File does not overwrite.".format("", os.path.basename(newPath), iDestination)
        else:          
            copy_file(iSource, iDestination)
        PrintCopySuccess()

def DoCopyAction(iAction, iSources, iDestinations, iIsOverwritten = False):
    print "Execute {}".format(iAction.attrib[XML_ATTRIBUTE_NAME])
    for source in iSources:
        if False == os.path.exists(source):
            print "{:3s}\"{}\" does not exist in \"{}\"".format("", os.path.basename(source), source)
            print os.path.isfile(source)
            continue
        for dest in iDestinations:
            CopyFolder(source, dest, iIsOverwritten)

def ExecuteAction(iRoot, iIsOverwritten = False):
    actions = iRoot.find(XML_ATTRIBUTE_ACTIONS)
    for action in actions:
        sources = GetActionSources(action)
        destinations = GetActionDestinations(action)
        DoCopyAction(action, sources, destinations, iIsOverwritten)

def GetRoot(fileName):
    tree = ET.parse(fileName)
    return tree.getroot()

def ArgumentsParser(iArgs):
    parser = argparse.ArgumentParser(prog = PROGRAM_NAME, conflict_handler = CONFLICT_HANDLER_RESOLVE, fromfile_prefix_chars = PARSER_PREFIX_CHAR)
    parser.add_argument("-f", "--file",
                        action="store",
                        dest="m_FileName",
                        required=True,
                        help="Input XML file name")
    parser.add_argument("-o", "--overwrite",
                        action="store_true",
                        dest="m_IsOverwritten",
                        default="False")               

    try:
        args, remaining_argv = parser.parse_known_args(iArgs)
    except IOError, msg:
        parser.error(str(msg))
    return args

# Main function
def Main(argv):
    if 1 <= len(argv):
        args = ArgumentsParser(argv)
    else:
        args = ArgumentsParser([PARSER_PREFIX_CHAR + CONFIG_FILE_NAME])

    fileName = args.m_FileName
    isOverwritten = args.m_IsOverwritten

    if True == os.path.exists(fileName):
        root = GetRoot(fileName)
        ExecuteAction(root, isOverwritten)
    else:
        print "{} does not exist.".format(fileName)
        sys.exit(0)

if __name__ == "__main__":
    Main(sys.argv[1:])
