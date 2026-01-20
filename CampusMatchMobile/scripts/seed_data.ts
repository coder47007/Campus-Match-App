
import { createClient } from '@supabase/supabase-js';
import fs from 'fs';
import path from 'path';

// Configuration (from config.ts)
const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);

// Images to upload (Absolute paths)
const IMAGES = [
    { name: 'female_student_1', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\female_student_1_1768778556120.png', gender: 'Female' },
    { name: 'female_student_2', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\female_student_2_1768778582002.png', gender: 'Female' },
    { name: 'female_student_3', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\female_student_3_1768778620702.png', gender: 'Female' },
    { name: 'male_student_1', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\male_student_1_1768778543032.png', gender: 'Male' },
    { name: 'male_student_2', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\male_student_2_1768778569343.png', gender: 'Male' },
    { name: 'male_student_3', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\male_student_3_1768778595591.png', gender: 'Male' },
    { name: 'male_student_4', path: 'C:\\Users\\Shams\\.gemini\\antigravity\\brain\\23f93232-e31f-4790-98dc-0689ac8873c1\\male_student_4_1768778634330.png', gender: 'Male' },
];

const mockData = {
    female_student_1: { name: 'Sophia Miller', age: 20, major: 'Psychology', bio: 'Coffee addict & future therapist ☕️' },
    female_student_2: { name: 'Emma Wilson', age: 19, major: 'Biology', bio: 'Nature lover 🌿 Hiking buddy needed!' },
    female_student_3: { name: 'Olivia Davis', age: 21, major: 'Fine Arts', bio: 'Always sketching. Let’s go to a museum! 🎨' },
    male_student_1: { name: 'Liam Johnson', age: 20, major: 'Computer Science', bio: 'Tech enthusiast & gamer 🎮' },
    male_student_2: { name: 'Noah Brown', age: 22, major: 'Business', bio: 'Crypto & chill 📈' },
    male_student_3: { name: 'William Jones', age: 21, major: 'English Lit', bio: 'Bookworm looking for a study date 📚' },
    male_student_4: { name: 'James Garcia', age: 19, major: 'Sports Management', bio: 'Always at the gym or the field 🏈' },
};

async function seedData() {
    console.log('Starting Supabase Seed...');

    // 1. Ensure bucket exists (Create if not)
    const { data: buckets, error: bucketError } = await supabase.storage.listBuckets();
    if (bucketError) console.error('Error listing buckets:', bucketError);

    let bucket = buckets?.find(b => b.name === 'avatars');
    if (!bucket) {
        console.log("Bucket 'avatars' not found, creating...");
        const { data: newBucket, error: createError } = await supabase.storage.createBucket('avatars', { public: true });
        if (createError) {
            console.error('Error creating bucket:', createError);
            // Fallback: Try to use it anyway, maybe it exists but permissions weird
        }
    } else {
        console.log("Bucket 'avatars' found.");
    }

    const createdStudents = [];

    for (const img of IMAGES) {
        console.log(`Processing ${img.name}...`);

        // 2. Upload Image
        const fileContent = fs.readFileSync(img.path);
        const fileName = `${Date.now()}_${path.basename(img.path)}`;

        const { data: uploadData, error: uploadError } = await supabase.storage
            .from('avatars')
            .upload(fileName, fileContent, {
                contentType: 'image/png',
                upsert: true
            });

        let publicUrl = '';

        if (uploadError) {
            console.error(`Failed to upload ${img.name}:`, uploadError.message);
            // Fallback placeholder if upload fails
            publicUrl = img.gender === 'Female'
                ? 'https://randomuser.me/api/portraits/women/44.jpg'
                : 'https://randomuser.me/api/portraits/men/32.jpg';
        } else {
            console.log(`Uploaded ${img.name} successfully.`);
            // Get Public URL
            const { data } = supabase.storage.from('avatars').getPublicUrl(fileName);
            publicUrl = data.publicUrl;
        }

        // 3. Insert Student Record
        const details = mockData[img.name as keyof typeof mockData];

        // Generate a random email to ensure uniqueness
        const uniqueSuffix = Math.floor(Math.random() * 10000);
        const email = `${details.name.split(' ')[0].toLowerCase()}${uniqueSuffix}@example.com`;

        const { data: student, error: insertError } = await supabase
            .from('Students')
            .insert({
                Email: email,
                Name: details.name,
                Age: details.age,
                Major: details.major,
                Bio: details.bio,
                Gender: img.gender,
                University: 'CampusMatch U',
                Year: 'Junior',
                PhotoUrl: publicUrl,
                Photos: [publicUrl], // Array of photos
                CreatedAt: new Date().toISOString()
            })
            .select()
            .single();

        if (insertError) {
            console.error(`Failed to create student ${details.name}:`, insertError.message);
        } else {
            console.log(`Created Student: ${student.Name} (ID: ${student.Id})`);
            createdStudents.push(student);
        }
    }

    console.log(`\nSuccessfully created ${createdStudents.length} students.`);
}

seedData();
