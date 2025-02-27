CREATE TABLE message_from_methodist
(
    id              UUID PRIMARY KEY,
    body            TEXT NOT NULL,
    sender_username TEXT NOT NULL
);

CREATE TABLE academic_group
(
    id   UUID PRIMARY KEY,
    name TEXT UNIQUE NOT NULL
);

CREATE TABLE student
(
    id        UUID PRIMARY KEY,
    full_name TEXT UNIQUE NOT NULL,
    group_id  UUID REFERENCES academic_group (id)
);

CREATE TABLE message_from_methodist_student
(
    message_id UUID REFERENCES message_from_methodist (id),
    student_id UUID REFERENCES student (id),
    PRIMARY KEY (message_id, student_id)
);

CREATE TABLE telegram_chat
(
    id         UUID PRIMARY KEY,
    title      TEXT UNIQUE   NOT NULL,
    ext_id     BIGINT UNIQUE NOT NULL,
    type       SMALLINT      NOT NULL,
    student_id UUID UNIQUE REFERENCES student (id),
    group_id   UUID UNIQUE REFERENCES academic_group (id)
);

CREATE TABLE telegram_message
(
    id             UUID PRIMARY KEY,
    ext_id         INT  NOT NULL,
    chat_id        UUID NOT NULL REFERENCES telegram_chat (id),
    src_message_id UUID NOT NULL REFERENCES message_from_methodist (id),
    UNIQUE (ext_id, chat_id)
);

CREATE TABLE telegram_message_reaction
(
    id               UUID PRIMARY KEY,
    reactor_username TEXT NOT NULL,
    reaction         TEXT NOT NULL,
    message_id       UUID NOT NULL REFERENCES telegram_message (id)
);