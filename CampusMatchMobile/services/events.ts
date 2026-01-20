import { supabase, getStudentId } from './supabase';

export interface CampusEvent {
    Id: number;
    CreatorId: number;
    Title: string;
    Location: string;
    Description?: string;
    StartTime: string;
    EndTime: string;
    CreatedAt: string;
    Creator?: {
        Name: string;
        PhotoUrl: string;
    };
    Attendees?: {
        StudentId: number;
    }[];
}

export const eventsApi = {
    // Get all active events
    getEvents: async (): Promise<CampusEvent[]> => {
        const now = new Date().toISOString();

        const { data, error } = await supabase
            .from('Events')
            .select(`
                *,
                Creator:Students(Name, PhotoUrl),
                Attendees:EventAttendees(StudentId)
            `)
            .gt('EndTime', now) // Only active events
            .order('StartTime', { ascending: true });

        if (error) {
            console.error('Error fetching events:', error);
            return [];
        }

        return data as CampusEvent[];
    },

    // Create a new event
    createEvent: async (
        title: string,
        location: string,
        durationMinutes: number
    ): Promise<CampusEvent | null> => {
        const studentId = await getStudentId();
        if (!studentId) return null;

        const startTime = new Date();
        const endTime = new Date(startTime.getTime() + durationMinutes * 60000);

        // 1. Insert Event
        const { data, error } = await supabase
            .from('Events')
            .insert({
                CreatorId: studentId,
                Title: title,
                Location: location,
                Description: '',        // Required by DB
                Category: 'General',    // Required by DB
                MaxAttendees: 100,      // Required by DB
                IsActive: true,         // Required by DB
                StartTime: startTime.toISOString(),
                EndTime: endTime.toISOString(),
                CreatedAt: startTime.toISOString()
            })
            .select('*, Creator:Students(Name)')
            .single();

        if (error) {
            console.error('Error creating event:', error);
            throw error;
        }

        // 2. Notify Matches (Background)
        // We do this asynchronously so we don't block the UI
        (async () => {
            try {
                // Find active matches
                const { data: matches } = await supabase
                    .from('Matches')
                    .select('Student1Id, Student2Id')
                    .or(`Student1Id.eq.${studentId},Student2Id.eq.${studentId}`)
                    .eq('IsActive', true);

                if (!matches?.length) return;

                // Collect IDs of people to notify
                const recipientIds = matches.map(m =>
                    m.Student1Id === studentId ? m.Student2Id : m.Student1Id
                );

                // Get their push tokens
                const { data: recipients } = await supabase
                    .from('Students')
                    .select('PushNotificationToken')
                    .in('Id', recipientIds)
                    .not('PushNotificationToken', 'is', null);

                if (!recipients?.length) return;

                // Send Push Notifications via Expo API
                const notifications = recipients.map(r => ({
                    to: r.PushNotificationToken,
                    sound: 'default',
                    title: `New Vibe: ${data.Creator.Name}`,
                    body: `${data.Creator.Name} is at ${location}: "${title}"`,
                    data: { eventId: data.Id, type: 'new_event' },
                }));

                await fetch('https://exp.host/--/api/v2/push/send', {
                    method: 'POST',
                    headers: {
                        'Accept': 'application/json',
                        'Accept-encoding': 'gzip, deflate',
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(notifications),
                });

                console.log(`Sent ${notifications.length} notifications for new event`);

            } catch (err) {
                console.error('Failed to send event notifications:', err);
            }
        })();

        return data;
    },

    // Delete my event
    deleteEvent: async (eventId: number): Promise<boolean> => {
        const studentId = await getStudentId();

        console.log(`[deleteEvent] Attempting to delete Event ${eventId} for Student ${studentId}`);

        const { error } = await supabase
            .from('Events')
            .delete()
            .eq('Id', eventId)
            .eq('CreatorId', studentId); // Ensure ownership at DB level

        if (error) {
            console.error('[deleteEvent] Error deleting event:', error);
            console.error('[deleteEvent] Message:', error.message);
            console.error('[deleteEvent] Details:', error.details);
            return false;
        }

        console.log('[deleteEvent] Successfully deleted event');
        return true;
    }
};
