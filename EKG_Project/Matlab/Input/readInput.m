clc
clear all

file = commaToDot('in.txt');
[fs, sig] = getInputData(file);

N = length(sig);
fs = double(fs);
t = linspace(0,(N/fs),N);
figure, plot(t', sig);
xlabel('czas[s]'), ylabel('amplituda [mV]');
title('EKG');