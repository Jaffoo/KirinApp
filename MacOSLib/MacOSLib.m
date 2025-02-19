#import <Cocoa/Cocoa.h>

typedef NS_ENUM(NSInteger, MsgResult){
    OK,
    Cancel,
    Yes,
    No
}

typedef NS_ENUM(NSInteger, MsgBtns) {
    OK,
    OKCancel,
    YesNo,
    YesNoCancel
};

typedef NS_ENUM(NSInteger, MessageType) {
    Info,
    Warning,
    Question,
    Error,
    Other
};

#ifdef __cplusplus
extern "C" {
#endif

// ���� C ����
void* CreateWindow(int left, int top, int width, int height, int chromeless);
void SetTitle(void* windowHandle, const char *title);
void SetIcon(void* windowHandle, const char *path);
void ShowWindow(void* windowHandle, int show);
void FocusWindow(void* windowHandle);
void RunLoop();
void Move(void* windowHandle, int left, int top);
void ChangeSize(void* windowHandle, int width, int height);
void TopMost(void* windowHandle, int topMost);
void Minimize(void* windowHandle);
void Maximize(void* windowHandle);

MsgResult ShowDialog(const char *title, const char *message, MsgBtns btn, MessageType messageType, const char *iconName)
#ifdef __cplusplus
}
#endif

// ������صı���
NSWindow *g_window = nil;

@interface MyWindowDelegate : NSObject <NSWindowDelegate>
@end

@implementation MyWindowDelegate
- (void)windowWillClose:(NSNotification *)notification {
    [NSApp terminate:nil];
}
@end

void* CreateWindow(int left, int top, int width, int height, int chromeless) {
    @autoreleasepool {
        NSRect frame = NSMakeRect(left, top, width, height);
        MyWindowDelegate *delegate = [[MyWindowDelegate alloc] init];

        NSUInteger styleMask = NSWindowStyleMaskTitled | NSWindowStyleMaskClosable | NSWindowStyleMaskResizable;
        if (chromeless) {
            styleMask = NSWindowStyleMaskBorderless; // �ޱ߿򴰿�
        }

        g_window = [[NSWindow alloc] initWithContentRect:frame
                                                styleMask:styleMask
                                                  backing:NSBackingStoreBuffered
                                                    defer:NO];
        [g_window setDelegate:delegate];
        return (__bridge void*)g_window; // ���ش��ھ��
    }
}

void SetTitle(void* windowHandle, const char *title) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        NSString *nsTitle = [NSString stringWithUTF8String:title];
        [window setTitle:nsTitle]; // ���ô��ڱ���
    }
}

void SetIcon(void* windowHandle, const char *path) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        NSImage *icon = [[NSImage alloc] initWithContentsOfFile:[NSString stringWithUTF8String:path]];
        if (icon) {
            [window setMiniwindowImage:icon]; // ������С������ͼ��
        } else {
            NSLog(@"�޷�����ͼ��·��: %s", path);
        }
    }
}

void ShowWindow(void* windowHandle, int show) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        if (show) {
            [window makeKeyAndOrderFront:nil]; // ��ʾ����
        } else {
            [window orderOut:nil]; // ���ش���
        }
    }
}

void FocusWindow(void* windowHandle) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        [window makeKeyAndOrderFront:nil]; // ʹ���ڻ�ý���
    }
}

void Move(void* windowHandle, int left, int top) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        NSRect frame = window.frame;
        frame.origin.x = left;
        frame.origin.y = top;
        [window setFrame:frame display:YES animate:YES]; // �ƶ�����
    }
}

void ChangeSize(void* windowHandle, int width, int height) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        NSRect frame = window.frame;
        frame.size.width = width;
        frame.size.height = height;
        [window setFrame:frame display:YES animate:YES]; // ���Ĵ��ڴ�С
    }
}

void TopMost(void* windowHandle, int topMost) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        [window setLevel:topMost ? NSStatusWindowLevel : NSNormalWindowLevel]; // ���ô����ö�
    }
}

void Minimize(void* windowHandle) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        [window miniaturize:nil]; // ��С������
    }
}

void Maximize(void* windowHandle) {
    NSWindow *window = (__bridge NSWindow *)windowHandle;
    if (window) {
        [window makeKeyAndOrderFront:nil]; // ȷ��������ǰ
        [window zoom:nil]; // ��󻯴���
    }
}

void RunLoop() {
    [[NSApplication sharedApplication] run]; // �����¼�ѭ��
}

NSImage* GetIcon(NSString *path) {
    NSImage *image = [[NSImage alloc] initWithContentsOfFile:path];
    if (!image) {
        NSLog(@"�޷�����ͼ��·��: %@", path);
    }
    return image;
}

MsgResult ShowDialog(const char *title, const char *message, MsgBtns btn, MessageType messageType, const char *iconName) {
    @autoreleasepool {
        NSAlert *alert = [[NSAlert alloc] init];
        [alert setMessageText:[NSString stringWithUTF8String:title]];
        [alert setInformativeText:[NSString stringWithUTF8String:message]];

        // ���ݰ�ť������Ӱ�ť
        if (btn == OK) {
            [alert addButtonWithTitle:@"ȷ��"];
        } else if (btn == OKCancel) {
            [alert addButtonWithTitle:@"ȷ��"];
            [alert addButtonWithTitle:@"ȡ��"];
        }else if (btn == YesNo) {
            [alert addButtonWithTitle:@"��"];
            [alert addButtonWithTitle:@"��"];
        }else if (btn == YesNoCancel) {
            [alert addButtonWithTitle:@"��"];
            [alert addButtonWithTitle:@"��"];
            [alert addButtonWithTitle:@"ȡ��"];
        }

        // ������Ϣ��������ͼ��
        if (messageType == Info) {
            [alert setIcon:[NSImage imageNamed:NSImageNameInfo]];
        } else if (messageType == Warning) {
            [alert setIcon:[NSImage imageNamed:NSImageNameWarning]];
        } else if (messageType == Error) {
            [alert setIcon:[NSImage imageNamed:NSImageNameError]];
        }else if(messageType == Question){
            [alert setIcon:[NSImage imageNamed:NSImageNameInfo]];
        }else{
            [alert setIcon:[NSImage imageNamed:NSImageNameInfo]];
        }

        if (iconName) {
            NSString *iconNameStr = [NSString stringWithUTF8String:iconName];
            [alert setIcon:[NSImage imageNamed:iconNameStr]];
        }

        // ��ʾ�Ի��򲢻�ȡ�û���Ӧ
        NSApplication.ModalResponse response = [alert runModal];

        // ������Ӧ������Ӧ��ö��ֵ
        switch (btn) {
            case OK:
                return OK;
            case OKCancel:
                return (response == NSAlertFirstButtonReturn) ? OK : Cancel;
            case YesNo:
                return (response == NSAlertFirstButtonReturn) ? Yes : No;
            case YesNoCancel:
                if (response == NSAlertFirstButtonReturn) return Yes; // ���ǡ�
                if (response == NSAlertSecondButtonReturn) return No; // ����
                return Cancel; // ��ȡ����
        }
        return OK;
    }
}