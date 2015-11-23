function [ fileOut ] = commaToDot( fileIn )

data = fileread(fileIn);
data = strrep(data, ',', '.');
fileOut = fopen(fileIn, 'w');
fwrite(fileOut, data);
fclose(fileOut);

end

