#import <Foundation/Foundation.h>
#import <Speech/Speech.h>

@interface SpeechRecognizer : NSObject <SFSpeechRecognizerDelegate>
@property (nonatomic, strong) SFSpeechRecognizer *speechRecognizer;
@property (nonatomic, strong) SFSpeechAudioBufferRecognitionRequest *recognitionRequest;
@property (nonatomic, strong) SFSpeechRecognitionTask *recognitionTask;
@property (nonatomic, strong) AVAudioEngine *audioEngine;
@property (nonatomic, copy) void(^recognitionCallback)(NSString *recognizedText);
@end

@implementation SpeechRecognizer

- (instancetype)init {
    self = [super init];
    if (self) {
        self.speechRecognizer = [[SFSpeechRecognizer alloc] initWithLocale:[NSLocale localeWithLocaleIdentifier:@"en-US"]];
        self.speechRecognizer.delegate = self;
        self.audioEngine = [[AVAudioEngine alloc] init];
    }
    return self;
}

- (BOOL)requestPermissions {
    return [SFSpeechRecognizer authorizationStatus] == SFSpeechRecognizerAuthorizationStatusAuthorized;
}

- (void)startRecognitionWithCallback:(void(^)(NSString *))callback {
    self.recognitionCallback = callback;
    
    if (self.recognitionTask) {
        [self.recognitionTask cancel];
        self.recognitionTask = nil;
    }
    
    AVAudioSession *audioSession = [AVAudioSession sharedInstance];
    NSError *error;
    [audioSession setCategory:AVAudioSessionCategoryRecord error:&error];
    [audioSession setMode:AVAudioSessionModeMeasurement error:&error];
    [audioSession setActive:YES withOptions:AVAudioSessionSetActiveOptionNotifyOthersOnDeactivation error:&error];
    
    self.recognitionRequest = [[SFSpeechAudioBufferRecognitionRequest alloc] init];
    self.recognitionRequest.shouldReportPartialResults = YES;
    
    AVAudioInputNode *inputNode = self.audioEngine.inputNode;
    
    self.recognitionTask = [self.speechRecognizer recognitionTaskWithRequest:self.recognitionRequest resultHandler:^(SFSpeechRecognitionResult * _Nullable result, NSError * _Nullable error) {
        BOOL isFinal = NO;
        
        if (result) {
            self.recognitionCallback(result.bestTranscription.formattedString);
            isFinal = result.isFinal;
        }
        
        if (error || isFinal) {
            [self.audioEngine stop];
            [inputNode removeTapOnBus:0];
            self.recognitionRequest = nil;
            self.recognitionTask = nil;
        }
    }];
    
    AVAudioFormat *recordingFormat = [inputNode outputFormatForBus:0];
    [inputNode installTapOnBus:0 bufferSize:1024 format:recordingFormat block:^(AVAudioPCMBuffer * _Nonnull buffer, AVAudioTime * _Nonnull when) {
        [self.recognitionRequest appendAudioPCMBuffer:buffer];
    }];
    
    [self.audioEngine prepare];
    [self.audioEngine startAndReturnError:&error];
}

- (void)stopRecognition {
    [self.audioEngine stop];
    [self.recognitionRequest endAudio];
}

@end

// C-style interface for Unity
extern "C" {
    static SpeechRecognizer *sharedInstance = nil;
    
    void _initSpeechRecognizer() {
        if (!sharedInstance) {
            sharedInstance = [[SpeechRecognizer alloc] init];
        }
    }
    
    bool _requestSpeechPermissions() {
        if (!sharedInstance) {
            _initSpeechRecognizer();
        }
        return [sharedInstance requestPermissions];
    }
    
    void _startSpeechRecognition(void(*callback)(const char*)) {
        if (!sharedInstance) {
            _initSpeechRecognizer();
        }
        
        [sharedInstance startRecognitionWithCallback:^(NSString *recognizedText) {
            if (callback) {
                const char *cString = [recognizedText UTF8String];
                callback(cString);
            }
        }];
    }
    
    void _stopSpeechRecognition() {
        if (sharedInstance) {
            [sharedInstance stopRecognition];
        }
    }
}


