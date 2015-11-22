function [ frequency, signal ] = getInputData( filename )
%file = commaToDot(filename);
fileID = fopen(filename);

frequency = textscan(fileID, '%u', 1);
frequency = frequency{1, 1};

signal = textscan(fileID, '%f','Delimiter', ' ');
signal = signal{1, 1};
signal = strrep(signal, ',', '.');

fclose(fileID);
end

