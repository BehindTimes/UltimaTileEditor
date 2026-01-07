using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Numerics;
using System.Text;

namespace UltimaTileEditor
{
    public struct Dict_entry
    {
        public byte root;
        public int codeword;
        public bool occupied;
    };

    internal class LzwDecompressor
    {
        const int S_SIZE = 10000;
        const int D_SIZE = 10000;
        const int max_codeword_length = 12;

        Dict_entry[] dict;
        byte[] mystack;

        int bits_read = 0;
        int dict_contains = 0;
        int stack_contains = 0;
        long position = 0;

        public LzwDecompressor()
        {
            dict = new Dict_entry[D_SIZE];
            mystack = new byte[S_SIZE];
        }

        // ----------------------------------------------
        // Read the next code word from the source buffer
        // ----------------------------------------------
        bool Get_next_codeword(out int codeword, byte[] source, int codeword_size)
        {
            byte b0 = 0;
            byte b1 = 0;
            byte b2 = 0;
            int xx, xy, xz;
            xx = bits_read / 8;

            b0 = source[bits_read / 8];
            if (source.Length > bits_read / 8 + 1)
            {
                b1 = source[bits_read / 8 + 1];
            }
            if (source.Length > bits_read / 8 + 2)
            {
                b2 = source[bits_read / 8 + 2];
            }
            xx = b2 << 16;
            xy = b1 << 8;
            xz = b0;
            codeword = xx + xy + xz;
            codeword = codeword >> (bits_read % 8);
            switch (codeword_size)
            {
                case 0x9:
                    codeword = codeword & 0x1ff;
                    break;
                case 0xa:
                    codeword = codeword & 0x3ff;
                    break;
                case 0xb:
                    codeword = codeword & 0x7ff;
                    break;
                case 0xc:
                    codeword = codeword & 0xfff;
                    break;
                default:
                    MessageBox.Show("Error: weird codeword size!");
                    return false;
            }
            bits_read += codeword_size;

            return true;
        }

        bool Get_next_codeword_u4(out int codeword, byte[] source)
        {
            int curByte = bits_read / 8;
            codeword = (source[bits_read / 8] << 8) + source[bits_read / 8 + 1];
            codeword = codeword >> (4 - (bits_read % 8));
            codeword = codeword & 0xfff;

            bits_read += 12;

            return true;
        }

        void Dict_init()
        {
            dict_contains = 0x102;
        }

        void Stack_init()
        {
            stack_contains = 0;
        }

        void Output_root(byte root, ref byte[] destination)
        {
            destination[position] = root;
            position++;
        }

        byte Dict_get_root(int codeword)
        {
            return (dict[codeword].root);
        }

        int Dict_get_codeword(int codeword)
        {
            return (dict[codeword].codeword);
        }

        bool Stack_is_empty()
        {
            return (stack_contains == 0);
        }

        bool Stack_is_full()
        {
            return (stack_contains == mystack.Length);
        }

        void Stack_push(byte element)
        {
            if (!Stack_is_full())
            {
                mystack[stack_contains] = element;
                stack_contains++;
            }
        }

        byte Stack_pop()
        {
            byte element;

            if (!Stack_is_empty())
            {
                element = mystack[stack_contains - 1];
                stack_contains--;
            }
            else
            {
                element = 0;
            }
            return element;
        }

        byte Stack_gettop()
        {
            if (!Stack_is_empty())
            {
                return (mystack[stack_contains - 1]);
            }
            return 0;
        }

        bool Get_string(int codeword)
        {
            byte root;
            int current_codeword;

            current_codeword = codeword;
            Stack_init();
            while (current_codeword > 0xff)
            {
                root = Dict_get_root(current_codeword);
                current_codeword = Dict_get_codeword(current_codeword);
                Stack_push(root);
            }

            // push the root at the leaf
            try
            {
                byte current_codeword_byte = Convert.ToByte(current_codeword);
                Stack_push(current_codeword_byte);
            }
            catch (OverflowException)
            {
                return false;
            }
            return true;
        }

        void Dict_add(byte root, int codeword)
        {
            dict[dict_contains].root = root;
            dict[dict_contains].codeword = codeword;
            dict_contains++;
        }

        // -----------------------------------------------------------------------------
        // LZW-decompress from buffer to buffer.
        // -----------------------------------------------------------------------------
        bool Lzw_decompressbuffer(byte[] source, ref byte[] destination)
        {
            bool end_marker_reached = false;
            int codeword_size = 9;
            int next_free_codeword = 0x102;
            int dictionary_size = 0x200;

            long bytes_written = 0;

            int cW;
            int pW = 0;
            byte C;
            byte poppedval;

            bits_read = 0;

            while (!end_marker_reached)
            {
                bool bValid = Get_next_codeword(out cW, source, codeword_size);
                if (!bValid)
                {
                    return false;
                }
                switch (cW)
                {
                    // re-init the dictionary
                    case 0x100:
                        codeword_size = 9;
                        next_free_codeword = 0x102;
                        dictionary_size = 0x200;
                        Dict_init();
                        bValid = Get_next_codeword(out cW, source, codeword_size);
                        if (!bValid)
                        {
                            return false;
                        }
                        position = bytes_written;
                        try
                        {
                            byte cWByte = Convert.ToByte(cW);
                            Output_root(cWByte, ref destination);
                        }
                        catch (OverflowException)
                        {
                            return false;
                        }
                        bytes_written = position;
                        break;
                    // end of compressed file has been reached
                    case 0x101:
                        end_marker_reached = true;
                        break;
                    // (cW <> 0x100) && (cW <> 0x101)
                    default:
                        if (cW < next_free_codeword)
                        {
                            // codeword is already in the dictionary
                            // create the string associated with cW (on the MyStack)
                            if (!Get_string(cW))
                            {
                                return false;
                            }
                            C = Stack_gettop();
                            // output the string represented by cW
                            while (!Stack_is_empty())
                            {
                                poppedval = Stack_pop();
                                position = bytes_written;
                                Output_root(poppedval, ref destination);
                                bytes_written = position;
                            }

                            // add pW+C to the dictionary
                            Dict_add(C, pW);

                            next_free_codeword++;
                            if (next_free_codeword >= dictionary_size)
                            {
                                if (codeword_size < max_codeword_length)
                                {
                                    codeword_size += 1;
                                    dictionary_size *= 2;
                                }
                            }
                        }
                        else
                        {
                            // codeword is not yet defined
                            // create the string associated with pW (on the MyStack)
                            Get_string(pW);
                            C = Stack_gettop();
                            // output the string represented by pW
                            while (!Stack_is_empty())
                            {
                                position = bytes_written;
                                Output_root(Stack_pop(), ref destination);
                                bytes_written = position;
                            }
                            // output the char C
                            position = bytes_written;
                            Output_root(C, ref destination);
                            bytes_written = position;
                            // the new dictionary entry must correspond to cW
                            // if it doesn't, something is wrong with the lzw-compressed data.
                            if (cW != next_free_codeword)
                            {
                                MessageBox.Show("cW != next_free_codeword!");
                                return false; // ERROR!  Come back later and close
                            }
                            // add pW+C to the dictionary
                            Dict_add(C, pW);

                            next_free_codeword++;
                            if (next_free_codeword >= dictionary_size)
                            {
                                if (codeword_size < max_codeword_length)
                                {
                                    codeword_size += 1;
                                    dictionary_size *= 2;
                                }
                            }
                        }
                        break;
                }
                // shift roles - the current cW becomes the new pW
                pW = cW;
            }
            return true;
        }

        // this function only checks a few *necessary* conditions
        // returns "false" if the file doesn't satisfy these conditions
        // return "true" otherwise
        public bool Is_valid_lzw_file(byte[] file_bytes)
        {
            // file must contain 4-byte size header and space for the 9-bit value 0x100
            if (file_bytes.Length < 6)
            {
                return false;
            }
            // the last byte of the size header must be 0 (U5's files aren't *that* big)
            if (file_bytes[3] != 0)
            {
                return false;
            }
            // the 9 bits after the size header must be 0x100
            if ((file_bytes[4] != 0) || ((file_bytes[5] & 1) != 1))
            {
                return false;
            }

            return true;
        }

        public int Get_uncompressed_size(byte[] file_bytes)
        {
            int uncompressed_file_length = BitConverter.ToInt32(file_bytes, 0);
            return uncompressed_file_length;
        }

        public bool Extract(byte[] file_bytes, out byte[]? outFile)
        {
            outFile = null;
            if (Is_valid_lzw_file(file_bytes))
            {
                // determine the buffer sizes
                int source_buffer_size = file_bytes.Length - 4;
                int destination_buffer_size = Get_uncompressed_size(file_bytes);

                // create the buffers
                byte[] source_buffer = new byte[source_buffer_size];
                byte[] destination_buffer = new byte[destination_buffer_size];

                Array.Copy(file_bytes, 4, source_buffer, 0, file_bytes.Length - 4);

                byte tempval = 255;
                Array.Fill(destination_buffer, tempval);

                // decompress the input file
                if (!Lzw_decompressbuffer(source_buffer, ref destination_buffer))
                {
                    return false;
                }

                outFile = destination_buffer;
            }
            return true;
        }

        public bool Compress(byte[] file_bytes, string outFile)
        {
            using (BinaryWriter binWriter =
                    new BinaryWriter(File.Open(outFile, FileMode.Create)))
            {
                binWriter.Write(file_bytes.Length);

                Build_dictionary(file_bytes, 0, binWriter);

            }

            return true;
        }

        byte code_word_remainder = 0;
        int code_word_shift = 0;

        void Write_code_word(int code_word, int code_word_size, BinaryWriter binWriter)
        {
            int temp_int = code_word << code_word_shift;
            temp_int += code_word_remainder;
            code_word_shift += code_word_size;
            while (code_word_shift >= 8)
            {
                byte temp_byte = (byte)(temp_int & 0xFF);
                binWriter.Write(temp_byte);
                temp_int >>= 8;
                code_word_shift -= 8;
            }
            code_word_remainder = (byte)(temp_int & 0xFF);
        }

        int Build_dictionary(byte[] file_bytes, int start_pos, BinaryWriter binWriter)
        {
            code_word_remainder = 0;
            code_word_shift = 0;
            int next_free_codeword = 0x102;
            int codeword_size = 9;
            List<List<byte>> cur_dict = new List<List<byte>>();
            int temp_pos = start_pos;
            bool invalid = false;

            Write_code_word(0x100, codeword_size, binWriter);

            while (temp_pos < file_bytes.Length)
            {
                int seq_index = 0;
                bool sequence_found = false;
                List<byte> temp_list = new List<byte>();
                temp_list.Add(file_bytes[temp_pos]);
                temp_pos++;
                if (temp_pos >= file_bytes.Length)
                {
                    // Write the code word
                    Write_code_word(temp_list[0], codeword_size, binWriter);
                    break; // We've reached the end, so just write this
                }
                temp_list.Add(file_bytes[temp_pos]);
                while (cur_dict.Any(p => p.SequenceEqual(temp_list)))
                {
                    seq_index = 0;
                    foreach (var curList in cur_dict)
                    {
                        if (curList.SequenceEqual(temp_list))
                        {
                            break;
                        }
                        seq_index++;
                    }

                    sequence_found = true;

                    temp_pos++;
                    if (temp_pos >= file_bytes.Length)
                    {
                        // Write the sequence
                        Write_code_word(next_free_codeword + seq_index, codeword_size, binWriter);
                        invalid = true;
                        break; // We've reached the end, so just write this
                    }
                    temp_list.Add(file_bytes[temp_pos]);
                }
                cur_dict.Add(temp_list);
                if (cur_dict.Count == 256 || cur_dict.Count == 768 || cur_dict.Count == 1792)
                {
                    codeword_size++;
                }
                else if (cur_dict.Count >= 3840)
                {
                    temp_pos -= (temp_list.Count - 1);
                    cur_dict.Clear();
                    Write_code_word(0x100, codeword_size, binWriter);
                    codeword_size = 9;
                    continue;
                }
                if (invalid)
                {
                    break;
                }
                if (sequence_found)
                {
                    // Write the sequence
                    Write_code_word(next_free_codeword + seq_index, codeword_size, binWriter);

                }
                else
                {
                    // Write the code word
                    Write_code_word(temp_list[0], codeword_size, binWriter);
                }
            }
            Write_code_word(0x101, codeword_size, binWriter);

            if (code_word_shift > 0)
            {
                binWriter.Write(code_word_remainder);
            }

            return -1;
        }

        #region Ultima 4 LZW
        // Original code from xu4 (https://xu4.sourceforge.net/)
        /*
        *  Copyright (C) 2002  Marc Winterrowd
        *
        *  This program is free software; you can redistribute it and/or modify
        *  it under the terms of the GNU General Public License as published by
        *  the Free Software Foundation; either version 2 of the License, or
        *  (at your option) any later version.
        *
        *  This program is distributed in the hope that it will be useful,
        *  but WITHOUT ANY WARRANTY; without even the implied warranty of
        *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
        *  GNU Library General Public License for more details.
        *
        *  You should have received a copy of the GNU General Public License
        *  along with this program; if not, write to the Free Software
        *  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
        */


        public bool MightBeValidCompressedFile(byte[] file_bytes)
        {
            if(file_bytes.Length == 0)
            {
                return false; 
            }
            bool c1 = (file_bytes.Length * 8) % 12 == 0;
            bool c2 = (file_bytes.Length * 8 - 4) % 12 == 0;
            bool c3 = c3 = (file_bytes[0] >> 4) == 0;
            // check if upper 4 bits are 0
            return ((c1 || c2) && c3);
        }

        //
        // This function returns the decompressed size of a block of compressed data.
        // It doesn't decompress the data.
        // Use this function if you want to decompress a block of data, but don't know the decompressed size
        // in advance.
        //
        // There is some error checking to detect if the compressed data is corrupt, but it's only rudimentary.
        // Returns:
        // No errors: (long) decompressed size
        // Error: (long) -1
        //
        int LzwGetDecompressedSize(byte[] file_bytes)
        {
            return GeneralizedDecompress(file_bytes, null);
        }

        // pushes the string associated with codeword onto the stack
        void Get_string_U4(int codeword, ref Dict_entry[] dictionary, ref byte[] lzwstack, ref int elementsInStack)
        {
            byte root;
            int currentCodeword = codeword;

            while(currentCodeword > 0xff)
            {
                root = dictionary[currentCodeword].root;
                currentCodeword = dictionary[currentCodeword].codeword;
                lzwstack[elementsInStack] = root;
                elementsInStack++;
            }
            lzwstack[elementsInStack] = (byte)currentCodeword;
            elementsInStack++;
        }

        private int GeneralizedDecompress(byte[] source, byte[]? outFile)
        {
            dict = new Dict_entry[D_SIZE];
            mystack = new byte[S_SIZE];

            bool unknownCodeword = false;
            int codewordsInDictionary = 0;
            const int maxDictEntries = 0xccc;

            int bytes_written = 0;

            // newpos: position in the dictionary where new codeword was added
            // must be equal to current codeword (if it isn't, the compressed data must be corrupt)
            // unknownCodeword: is the current codeword in the dictionary?
            int newpos;

            int old_code;
            int new_code;

            byte character;
            int elementsInStack = 0;

            bits_read = 0;

            for (int i = 0; i < 0x100; i++)
            {
                dict[i].occupied = true;
            }

            if (bits_read + 12 <= source.Length * 8)
            {
                bool bValid = Get_next_codeword_u4(out old_code, source);
                character = (byte)old_code;
                if (outFile != null)
                {
                    outFile[bytes_written] = character;
                }
                bytes_written++;

                while (bits_read + 12 <= source.Length * 8) // WHILE there are still input characters DO
                {
                    bValid = Get_next_codeword_u4(out new_code, source);
                    if (!bValid)
                    {
                        return -1;
                    }

                    if (dict[new_code].occupied)
                    {
                        /* codeword is present in the dictionary                                               */
                        /* it must either be a root or a non-root that has already been added to the dicionary */
                        unknownCodeword = false;

                        /* STRING = get translation of NEW_CODE */
                        Get_string_U4(new_code, ref dict, ref mystack, ref elementsInStack);
                    }
                    else
                    {
                        /* codeword is yet to be defined */
                        unknownCodeword = true;

                        mystack[elementsInStack] = character;
                        elementsInStack++;

                        Get_string_U4(old_code, ref dict, ref mystack, ref elementsInStack);
                    }

                    // CHARACTER = first character in STRING
                    character = mystack[elementsInStack - 1];   // element at top of stack

                    // output STRING
                    while (elementsInStack > 0)
                    {
                        if (outFile != null)
                        {
                            outFile[bytes_written] = mystack[elementsInStack - 1];
                        }
                        bytes_written++;
                        elementsInStack--;
                    }

                    // add OLD_CODE + CHARACTER to the translation table
                    newpos = GetNewHashCode(character, old_code, ref dict);

                    dict[newpos].root = character;
                    dict[newpos].codeword = old_code;
                    dict[newpos].occupied = true;
                    codewordsInDictionary++;

                    /* check for errors */
                    if (unknownCodeword && (newpos != new_code))
                    {
                        /* clean up */
                        return (-1);
                    }

                    if (codewordsInDictionary > maxDictEntries)
                    {
                        /* wipe dictionary */
                        codewordsInDictionary = 0;
                        dict = new Dict_entry[D_SIZE];
                        for (int i = 0; i < 0x100; i++)
                        {
                            dict[i].occupied = true;
                        }

                        if (bits_read + 12 <= source.Length * 8)
                        {
                            bValid = Get_next_codeword_u4(out new_code, source);
                            if (!bValid)
                            {
                                return -1;
                            }
                            character = (byte)new_code;

                            if (outFile != null)
                            {
                                outFile[bytes_written] = character;
                            }

                            bytes_written++;
                        }
                        else
                        {
                            /* clean up */
                            return (bytes_written);
                        }
                    }

                    /* OLD_CODE = NEW_CODE */
                    old_code = new_code;
                }
            }
            return bytes_written;
        }

        private int GetNewHashCode(byte root, int codeword, ref Dict_entry[] dictionary)
        {
            int hashCode = 0;

            // probe 1
            hashCode = Probe1(root, codeword);
            if (HashPosFound(hashCode, root, codeword, ref dictionary))
            {
                return (hashCode);
            }
            // probe 2
            hashCode = Probe2(root, codeword);
            if (HashPosFound(hashCode, root, codeword, ref dictionary))
            {
                return (hashCode);
            }
            // probe 3
            do
            {
                hashCode = Probe3(hashCode);
            } while (!HashPosFound(hashCode, root, codeword, ref dictionary));

            return hashCode;
        }

        bool HashPosFound(int hashCode, byte root, int codeword, ref Dict_entry[] dictionary)
        {
            if (hashCode > 0xff)   /* hash codes must not be roots */
            {
                bool c1 = false;
                bool c2 = false;
                bool c3 = false;

                if (dictionary[hashCode].occupied)
                {
                    /* hash table position is occupied */
                    c1 = true;
                    /* is our (root,codeword) pair already in the hash table? */
                    c2 = dictionary[hashCode].root == root;
                    c3 = dictionary[hashCode].codeword == codeword;
                }
                else
                {
                    /* hash table position is free */
                    c1 = false;
                }

                return ((!c1) || (c1 && c2 && c3));
            }
            else
            {
                return false;
            }
        }

        int Probe1(byte root, int codeword)
        {
            int newHashCode = ((root << 4) ^ codeword) & 0xfff;
            return (newHashCode);
        }

        /* The secondary probe uses some assembler instructions that aren't easily translated to C. */
        int Probe2(byte root, int codeword)
        {
            /* registers[0] == AX, registers[1] == DX */
            int[] registers = new int[2];
            int temp;
            int carry, oldCarry;
            int i, j;

            /* the pre-mul part */
            registers[1] = 0;
            registers[0] = ((root << 1) + codeword) | 0x800;

            /* the mul part (simulated mul instruction) */
            /* DX:AX = AX * AX                          */
            temp = (registers[0] & 0xff) * (registers[0] & 0xff);
            temp += 2 * (registers[0] & 0xff) * (registers[0] >> 8) * 0x100;
            registers[1] = (temp >> 16) + (registers[0] >> 8) * (registers[0] >> 8);
            registers[0] = temp & 0xffff;

            /* if DX != 0, the mul instruction sets the carry flag */
            if (registers[1] == 00) { carry = 0; }
            else { carry = 1; }

            /* the rcl part */
            for (i = 0; i < 2; i++)   /* 2 rcl's */
            {
                for (j = 0; j < 2; j++)   /* rotate through 2 registers */
                {
                    oldCarry = carry;
                    carry = (registers[j] >> 15) & 1;
                    registers[j] = (registers[j] << 1) | oldCarry;
                    registers[j] = registers[j] & 0xffff;   /* make sure register stays 16 bit */
                }
            }

            /* final touches */
            registers[0] = ((registers[0] >> 8) | (registers[1] << 8)) & 0xfff;

            return ((int)registers[0]);
        }

        int Probe3(int hashCode)
        {
            const int probeOffset = 0x1fd;   /* I think 0x1fd is prime */

            int newHashCode = (hashCode + probeOffset) & 0xfff;
            return ((int)newHashCode);
        }

        public bool ExtractU4(byte[] file_bytes, out byte[]? outFile)
        {
            outFile = null;
            if(!MightBeValidCompressedFile(file_bytes))
            {
                return false;
            }
            int decompressed_filesize = LzwGetDecompressedSize(file_bytes);
            if (decompressed_filesize <= 0)
            {
                return false;
            }

            outFile = new byte[decompressed_filesize];
            GeneralizedDecompress(file_bytes, outFile);

            return true;
        }

        void Write_code_word_U4(int code_word, BinaryWriter binWriter)
        {
            int temp_int = 0;
            if (code_word_shift == 0)
            {
                temp_int = (code_word >> 4) & 0xFF;
                binWriter.Write((byte)temp_int);
                code_word_remainder = (byte)(code_word & 0xF);
                code_word_shift = 4;
            }
            else
            {
                temp_int = (code_word_remainder << 4) & 0xF0;
                temp_int += ((code_word >> 8) & 0xF);
                binWriter.Write((byte)temp_int);
                temp_int = code_word & 0xFF;
                binWriter.Write((byte)temp_int);
                code_word_remainder = 0;
                code_word_shift = 0;
            }
        }

        void Write_code_word_U4_finish(int code_word, BinaryWriter binWriter)
        {
            int temp_int = 0;
            if (code_word_shift == 0)
            {
                temp_int = (code_word >> 4) & 0xFF;
                binWriter.Write((byte)temp_int);
                code_word_remainder = (byte)(code_word & 0xF);
                code_word_shift = 4;
                temp_int = (code_word << 4) & 0xF0;
                binWriter.Write((byte)temp_int);
            }
            else
            {
                temp_int = (code_word_remainder << 4) & 0xF0;
                temp_int += ((code_word >> 8) & 0xF);
                binWriter.Write((byte)temp_int);
                temp_int = code_word & 0xFF;
                binWriter.Write((byte)temp_int);
                code_word_remainder = 0;
                code_word_shift = 0;
            }
        }

        int Build_dictionary_U4(byte[] file_bytes, int start_pos, BinaryWriter binWriter)
        {
            int temp_pos = start_pos;
            const int maxDictEntries = 0xccc;
            dict = new Dict_entry[D_SIZE];
            Dictionary<List<byte>, int> dict_sequences = new Dictionary<List<byte>, int>();
            List<byte> curSequence = new List<byte>();
            bool has_remainder = false;
            int codewordsInDictionary = 0;

            if (file_bytes.Length <= 0)
            {
                return -1;
            }

            for (int i = 0; i < 0x100; i++)
            {
                dict[i].occupied = true;
                curSequence = new List<byte>();
                curSequence.Add((byte)i);
                dict_sequences.Add(curSequence, i);
            }
           
            curSequence = new List<byte>();
            curSequence.Add(file_bytes[temp_pos]);
            temp_pos++;

            List<byte> tempSequence = new List<byte>(curSequence);

            while (temp_pos < file_bytes.Length)
            {
                tempSequence.Add(file_bytes[temp_pos]);
                if (dict_sequences.Keys.Any(p => p.SequenceEqual(tempSequence)))
                {
                    curSequence = new List<byte>(tempSequence);
                    temp_pos++;
                    has_remainder = true;
                    continue;
                }
                else
                {
                    has_remainder = false;
                    var match = dict_sequences.Keys.First(p => p.SequenceEqual(curSequence));
                    if (!dict_sequences.ContainsKey(match))
                    {
                        MessageBox.Show("Corrupt sequence");
                        return -1;
                    }

                    int code_word = dict_sequences[match];

                    Write_code_word_U4(code_word, binWriter);

                    byte root = curSequence[0];
                    byte root1 = tempSequence.Last();
                    int new_code = GetNewHashCode(root1, code_word, ref dict);
                    dict[new_code].occupied = true;
                    dict[new_code].root = root1;
                    dict[new_code].codeword = code_word;
                    dict_sequences.Add(tempSequence, new_code);
                    tempSequence = new List<byte>();
                    tempSequence.Add(file_bytes[temp_pos]);
                    curSequence = new List<byte>(tempSequence);

                    codewordsInDictionary++;
                    if (codewordsInDictionary > maxDictEntries)
                    {
                        match = dict_sequences.Keys.First(p => p.SequenceEqual(curSequence));
                        code_word = dict_sequences[match];
                        Write_code_word_U4(code_word, binWriter);
                        temp_pos++;
                        if(temp_pos >= file_bytes.Length)
                        {
                            return -1;
                        }
                        tempSequence = new List<byte>();
                        tempSequence.Add(file_bytes[temp_pos]);
                        curSequence = new List<byte>(tempSequence);

                        /* wipe dictionary */
                        dict_sequences = new Dictionary<List<byte>, int>();
                        codewordsInDictionary = 0;
                        dict = new Dict_entry[D_SIZE];
                        for (int i = 0; i < 0x100; i++)
                        {
                            dict[i].occupied = true;
                            curSequence = new List<byte>();
                            curSequence.Add((byte)i);
                            dict_sequences.Add(curSequence, i);
                        }

                        curSequence = new List<byte>();
                        curSequence.Add(file_bytes[temp_pos]);
                    }

                    temp_pos++;
                }
            }
            if(has_remainder)
            {
                var match = dict_sequences.Keys.First(p => p.SequenceEqual(curSequence));
                if (!dict_sequences.ContainsKey(match))
                {
                    MessageBox.Show("Corrupt sequence");
                    return -1;
                }
                int code_word = dict_sequences[match];
                Write_code_word_U4_finish(code_word, binWriter);
            }
            return -1;
        }

        public bool CompressU4Lzw(byte[] file_bytes, string outFile)
        {
            using (BinaryWriter binWriter =
                    new BinaryWriter(File.Open(outFile, FileMode.Create)))
            {
                Build_dictionary_U4(file_bytes, 0, binWriter);
            }

            return true;
        }
        #endregion
    }
}
